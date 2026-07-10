using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Imports;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Domain.Enums;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Utilities;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace CampusERP.Infrastructure.Services;

public class StudentImportService : IStudentImportService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IPasswordService _passwordService;

    public StudentImportService(ApplicationDbContext dbContext, IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    public async Task<byte[]> GenerateTemplateAsync()
    {
        using var workbook = new XLWorkbook();

        var sheet = workbook.Worksheets.Add("Students");

        sheet.Cell(1, 1).Value = "Admission Number";
        sheet.Cell(1, 2).Value = "Roll Number";
        sheet.Cell(1, 3).Value = "First Name";
        sheet.Cell(1, 4).Value = "Last Name";
        sheet.Cell(1, 5).Value = "Email";
        sheet.Cell(1, 6).Value = "Phone Number";
        sheet.Cell(1, 7).Value = "Admission Date";
        sheet.Cell(1, 8).Value = "Department";
        sheet.Cell(1, 9).Value = "Course";
        sheet.Cell(1, 10).Value = "Semester";
        sheet.Cell(1, 11).Value = "Section";

        sheet.Row(1).Style.Font.Bold = true;

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }

    private List<StudentImportRow> ReadExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        using var workbook = new XLWorkbook(stream);

        var sheet = workbook.Worksheet(1);

        var rows = new List<StudentImportRow>();

        var currentRow = 2;

        while (!sheet.Cell(currentRow, 1).IsEmpty())
        {
            var admissionDateCell = sheet.Cell(currentRow, 7);

            DateOnly admissionDate;

            if (admissionDateCell.DataType == XLDataType.DateTime)
            {
                admissionDate = DateOnly.FromDateTime(admissionDateCell.GetDateTime());
            }
            else
            {
                var dateText = admissionDateCell.GetString().Trim();

                if (!DateOnly.TryParse(dateText, out admissionDate))
                {
                    throw new Exception(
                        $"Invalid Admission Date '{dateText}' at row {currentRow}."
                    );
                }
            }

            rows.Add(new StudentImportRow
            {
                RowNumber = currentRow,

                AdmissionNumber = sheet.Cell(currentRow, 1).GetString().Trim(),

                RollNumber = sheet.Cell(currentRow, 2).GetString().Trim(),

                FirstName = sheet.Cell(currentRow, 3).GetString().Trim(),

                LastName = sheet.Cell(currentRow, 4).GetString().Trim(),

                Email = sheet.Cell(currentRow, 5).GetString().Trim(),

                PhoneNumber = sheet.Cell(currentRow, 6).GetString().Trim(),

                AdmissionDate = admissionDate,

                Department = sheet.Cell(currentRow, 8).GetString().Trim(),

                Course = sheet.Cell(currentRow, 9).GetString().Trim(),

                Semester = sheet.Cell(currentRow, 10).GetString().Trim(),

                Section = sheet.Cell(currentRow, 11).GetString().Trim()
            });

            currentRow++;
        }

        return rows;
    }

    public async Task<StudentImportValidationResponse> ValidateAsync(ImportStudentsRequest request)
    {
        var result = await ValidateInternalAsync(request);

        return result.Validation;
    }

    public async Task<StudentImportValidationResponse> ImportAsync(ImportStudentsRequest request)
    {
        var result = await ValidateInternalAsync(request);

        if (!result.Validation.CanImport)
        {
            return result.Validation;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var users = new List<User>();

            var students = new List<Student>();

            var enrollments = new List<StudentEnrollment>();

            var userRoles = new List<UserRole>();

            var studentRoleId = await _dbContext.Roles
                .Where(x => x.Name == "Student")
                .Select(x => x.Id)
                .FirstAsync();

            var academicSessionId = await _dbContext.AcademicSessions
                .Where(x =>
                    x.InstitutionId == request.InstitutionId &&
                    x.CampusId == request.CampusId &&
                    x.IsCurrent)
                .Select(x => x.Id)
                .FirstAsync();

            var departments = await _dbContext.Departments
                .Where(x =>
                    x.InstitutionId == request.InstitutionId &&
                    x.CampusId == request.CampusId)
                .ToDictionaryAsync(
                    x => x.Name.Trim().ToUpperInvariant());

            var courseLookup = await _dbContext.Courses
                .ToDictionaryAsync(
                    x => (
                        x.DepartmentId,
                        x.Name.Trim().ToUpperInvariant()));

            var semesterLookup = await _dbContext.Semesters
                .ToDictionaryAsync(
                    x => (
                        x.CourseId,
                        x.Name.Trim().ToUpperInvariant()));

            var sectionLookup = await _dbContext.Sections
                .ToDictionaryAsync(
                    x => (
                        x.SemesterId,
                        x.Name.Trim().ToUpperInvariant()));

            foreach (var row in result.Rows)
            {
                if (!departments.TryGetValue(row.Department.Trim().ToUpperInvariant(), out var department))
                {
                    throw new Exception($"Department '{row.Department}' not found.");
                }

                if (!courseLookup.TryGetValue((department.Id, row.Course.Trim().ToUpperInvariant()), out var course))
                {
                    throw new Exception($"Course '{row.Course}' not found.");
                }

                if (!semesterLookup.TryGetValue((course.Id, row.Semester.Trim().ToUpperInvariant()), out var semester))
                {
                    throw new Exception($"Semester '{row.Semester}' not found.");
                }

                if (!sectionLookup.TryGetValue((semester.Id, row.Section.Trim().ToUpperInvariant()), out var section))
                {
                    throw new Exception( $"Section '{row.Section}' not found.");
                }

                var password = PasswordGenerator.Generate();

                var user = new User
                {
                    Id = Guid.NewGuid(),

                    InstitutionId = request.InstitutionId,

                    CampusId = request.CampusId,

                    FirstName = row.FirstName,

                    LastName = row.LastName,

                    Email = row.Email,

                    PhoneNumber = row.PhoneNumber,

                    PasswordHash = _passwordService.HashPassword(password),

                    IsActive = true
                };

                users.Add(user);

                var student = new Student
                {
                    Id = Guid.NewGuid(),

                    UserId = user.Id,

                    InstitutionId = request.InstitutionId,

                    CampusId = request.CampusId,

                    AdmissionNumber = row.AdmissionNumber,

                    RollNumber = row.RollNumber,

                    Batch = $"{row.AdmissionDate.Year}-{row.AdmissionDate.Year + course.DurationYears}",

                    AdmissionDate = row.AdmissionDate.ToDateTime(TimeOnly.MinValue),

                    IsActive = true
                };

                students.Add(student);

                enrollments.Add(new StudentEnrollment
                {
                    Id = Guid.NewGuid(),

                    StudentId = student.Id,

                    InstitutionId = request.InstitutionId,

                    CampusId = request.CampusId,

                    DepartmentId = department.Id,

                    CourseId = course.Id,

                    SemesterId = semester.Id,

                    SectionId = section.Id,

                    AcademicSessionId = academicSessionId,

                    EnrollmentStatus = EnrollmentStatus.Active,

                    PromotionStatus = PromotionStatus.NewAdmission,

                    IsCurrent = true
                });

                userRoles.Add(new UserRole
                {
                    Id = Guid.NewGuid(),

                    UserId = user.Id,

                    RoleId = studentRoleId
                });

                result.Validation.Credentials.Add(new StudentImportCredential
                {
                    AdmissionNumber = row.AdmissionNumber,
                    StudentName = $"{row.FirstName} {row.LastName}",
                    Email = row.Email,
                    TemporaryPassword = password
                });
            }

            _dbContext.Users.AddRange(users);

            _dbContext.Students.AddRange(students);

            _dbContext.StudentEnrollments.AddRange(enrollments);

            _dbContext.UserRoles.AddRange(userRoles);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return result.Validation;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private Task ValidateRequiredFields(List<StudentImportRow> rows, StudentImportValidationResponse response)
    {
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.AdmissionNumber))
                AddError(response, row.RowNumber, "Admission Number", "Admission Number is required.");

            if (string.IsNullOrWhiteSpace(row.RollNumber))
                AddError(response, row.RowNumber, "Roll Number", "Roll Number is required.");

            if (string.IsNullOrWhiteSpace(row.FirstName))
                AddError(response, row.RowNumber, "First Name", "First Name is required.");

            if (string.IsNullOrWhiteSpace(row.LastName))
                AddError(response, row.RowNumber, "Last Name", "Last Name is required.");

            if (string.IsNullOrWhiteSpace(row.Email))
                AddError(response, row.RowNumber, "Email", "Email is required.");

            if (string.IsNullOrWhiteSpace(row.Department))
                AddError(response, row.RowNumber, "Department", "Department is required.");

            if (string.IsNullOrWhiteSpace(row.Course))
                AddError(response, row.RowNumber, "Course", "Course is required.");

            if (string.IsNullOrWhiteSpace(row.Semester))
                AddError(response, row.RowNumber, "Semester", "Semester is required.");

            if (string.IsNullOrWhiteSpace(row.Section))
                AddError(response, row.RowNumber, "Section", "Section is required.");
        }

        return Task.CompletedTask;
    }

    private static void AddError(StudentImportValidationResponse response, int rowNumber, string column, string message)
    {
        response.Errors.Add(new StudentImportError
        {
            RowNumber = rowNumber,

            Column = column,

            Message = message
        });
    }

    private Task ValidateDuplicateRows(List<StudentImportRow> rows, StudentImportValidationResponse response)
    {
        foreach (var group in rows.GroupBy(x => x.AdmissionNumber))
        {
            if (string.IsNullOrWhiteSpace(group.Key))
                continue;

            if (group.Count() <= 1)
                continue;

            foreach (var row in group)
            {
                AddError(response,row.RowNumber,"Admission Number","Duplicate Admission Number in Excel.");
            }
        }

        foreach (var group in rows.GroupBy(x => x.RollNumber))
        {
            if (string.IsNullOrWhiteSpace(group.Key))
                continue;

            if (group.Count() <= 1)
                continue;

            foreach (var row in group)
            {
                AddError(response,row.RowNumber,"Roll Number","Duplicate Roll Number in Excel.");
            }
        }

        foreach (var group in rows.GroupBy(x => x.Email.ToLower()))
        {
            if (string.IsNullOrWhiteSpace(group.Key))
                continue;

            if (group.Count() <= 1)
                continue;

            foreach (var row in group)
            {
                AddError(response,row.RowNumber,"Email","Duplicate Email in Excel.");
            }
        }

        return Task.CompletedTask;
    }

    private async Task ValidateDatabaseDuplicates(List<StudentImportRow> rows, StudentImportValidationResponse response)
    {
        var admissionNumbers = rows
            .Select(x => x.AdmissionNumber)
            .ToList();

        var rollNumbers = rows
            .Select(x => x.RollNumber)
            .ToList();

        var emails = rows
            .Select(x => x.Email)
            .ToList();

        var existingStudents = await _dbContext.Students
            .Where(x =>
                admissionNumbers.Contains(x.AdmissionNumber) ||
                rollNumbers.Contains(x.RollNumber))
            .ToListAsync();

        var existingUsers = await _dbContext.Users
            .Where(x => emails.Contains(x.Email))
            .ToListAsync();

        foreach (var row in rows)
        {
            if (existingStudents.Any(x => x.AdmissionNumber == row.AdmissionNumber))
            {
                AddError(
                    response,
                    row.RowNumber,
                    "Admission Number",
                    "Admission Number already exists.");
            }

            if (existingStudents.Any(x => x.RollNumber == row.RollNumber))
            {
                AddError(
                    response,
                    row.RowNumber,
                    "Roll Number",
                    "Roll Number already exists.");
            }

            if (existingUsers.Any(x => x.Email == row.Email))
            {
                AddError(
                    response,
                    row.RowNumber,
                    "Email",
                    "Email already exists.");
            }
        }
    }

    private async Task ValidateHierarchy(List<StudentImportRow> rows, ImportStudentsRequest request, StudentImportValidationResponse response)
    {
        var departments = await _dbContext.Departments
            .Where(x =>
                x.InstitutionId == request.InstitutionId &&
                x.CampusId == request.CampusId)
            .ToListAsync();

        var courses = await _dbContext.Courses
            .Where(x =>
                x.InstitutionId == request.InstitutionId &&
                x.CampusId == request.CampusId)
            .ToListAsync();

        var semesters = await _dbContext.Semesters
            .Where(x =>
                x.InstitutionId == request.InstitutionId &&
                x.CampusId == request.CampusId)
            .ToListAsync();

        var sections = await _dbContext.Sections
            .Where(x =>
                x.InstitutionId == request.InstitutionId &&
                x.CampusId == request.CampusId)
            .ToListAsync();

        foreach (var row in rows)
        {
            var department = departments.FirstOrDefault(x =>
                x.Name.Equals(row.Department, StringComparison.OrdinalIgnoreCase));

            if (department is null)
            {
                AddError(response,
                    row.RowNumber,
                    "Department",
                    "Department not found.");

                continue;
            }

            var course = courses.FirstOrDefault(x =>
                x.DepartmentId == department.Id &&
                x.Name.Equals(row.Course, StringComparison.OrdinalIgnoreCase));

            if (course is null)
            {
                AddError(response,
                    row.RowNumber,
                    "Course",
                    "Course not found under selected Department.");

                continue;
            }

            var semester = semesters.FirstOrDefault(x =>
                x.CourseId == course.Id &&
                x.Name.Equals(row.Semester, StringComparison.OrdinalIgnoreCase));

            if (semester is null)
            {
                AddError(response,
                    row.RowNumber,
                    "Semester",
                    "Semester not found under selected Course.");

                continue;
            }

            var section = sections.FirstOrDefault(x =>
                x.SemesterId == semester.Id &&
                x.Name.Equals(row.Section, StringComparison.OrdinalIgnoreCase));

            if (section is null)
            {
                AddError(response,
                    row.RowNumber,
                    "Section",
                    "Section not found under selected Semester.");
            }
        }
    }

    private Task ValidatePhoneNumbers(List<StudentImportRow> rows, StudentImportValidationResponse response)
    {
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.PhoneNumber))
                continue;

            if (!Regex.IsMatch(row.PhoneNumber, @"^[6-9]\d{9}$"))
            {
                AddError(response,
                    row.RowNumber,
                    "Phone Number",
                    "Invalid phone number.");
            }
        }

        return Task.CompletedTask;
    }

    private Task ValidateEmails(List<StudentImportRow> rows, StudentImportValidationResponse response)
    {
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Email))
                continue;

            try
            {
                _ = new MailAddress(row.Email);
            }
            catch
            {
                AddError(
                    response,
                    row.RowNumber,
                    "Email",
                    "Invalid email address.");
            }
        }

        return Task.CompletedTask;
    }

    private async Task<(List<StudentImportRow> Rows, StudentImportValidationResponse Validation)> ValidateInternalAsync(ImportStudentsRequest request)
    {
        var rows = ReadExcel(request.File);

        var response = new StudentImportValidationResponse
        {
            TotalRows = rows.Count
        };

        await ValidateRequiredFields(rows, response);

        await ValidateDuplicateRows(rows, response);

        await ValidateDatabaseDuplicates(rows, response);

        await ValidateHierarchy(rows, request, response);

        await ValidateEmails(rows, response);

        await ValidatePhoneNumbers(rows, response);

        foreach (var row in rows)
        {
            response.Preview.Add(new StudentImportPreviewRow
            {
                RowNumber = row.RowNumber,

                AdmissionNumber = row.AdmissionNumber,

                StudentName = $"{row.FirstName} {row.LastName}",

                Department = row.Department,

                Course = row.Course,

                Semester = row.Semester,

                Section = row.Section,

                IsValid = !response.Errors.Any(x => x.RowNumber == row.RowNumber)
            });
        }

        response.ValidRows = response.Preview.Count(x => x.IsValid);

        response.InvalidRows = response.TotalRows - response.ValidRows;

        return (rows, response);
    }

    public byte[] GenerateCredentialsExcel(IEnumerable<StudentImportCredential> credentials)
    {
        using var workbook = new XLWorkbook();

        var sheet = workbook.Worksheets.Add("Credentials");

        sheet.Cell(1, 1).Value = "Admission Number";
        sheet.Cell(1, 2).Value = "Student";
        sheet.Cell(1, 3).Value = "Email";
        sheet.Cell(1, 4).Value = "Temporary Password";

        sheet.Row(1).Style.Font.Bold = true;

        var row = 2;

        foreach (var credential in credentials)
        {
            sheet.Cell(row, 1).Value = credential.AdmissionNumber;
            sheet.Cell(row, 2).Value = credential.StudentName;
            sheet.Cell(row, 3).Value = credential.Email;
            sheet.Cell(row, 4).Value = credential.TemporaryPassword;

            row++;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }
}