using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    private readonly IPasswordService _passwordService;

    public ProfileService(ApplicationDbContext dbContext, IDataAccessScope scope, IPasswordService passwordService)
    {
        _dbContext = dbContext;

        _scope = scope;

        _passwordService = passwordService;
    }

    public async Task<ProfileResponse> GetMyProfileAsync()
    {
        var userId = _scope.UserId();

        var user = await _dbContext.Users
                .Include(x => x.Institution)
                .Include(x => x.Campus)
                .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new Exception("User not found.");
        }

        var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.UserId == userId);

        if (student != null)
        {
            var enrollment = await _dbContext.StudentEnrollments
                                .Include(x => x.AcademicSession)
                                .Include(x => x.Department)
                                .Include(x => x.Course)
                                .Include(x => x.Semester)
                                .Include(x => x.Section)
                                .FirstOrDefaultAsync(x => x.StudentId == student.Id && x.IsCurrent);

            return new ProfileResponse
            {
                UserId = user.Id,

                StudentId = student.Id,

                InstitutionId = student.InstitutionId,

                CampusId = student.CampusId,

                InstitutionName = user.Institution.Name,

                CampusName = user.Campus.Name,

                FullName = string.Join(" ", user.FirstName, user.LastName),

                Email = user.Email,

                PhoneNumber = user.PhoneNumber,

                Role = RoleConstants.Student,

                IsActive = student.IsActive,

                ProfilePhotoUrl = user.ProfilePhotoUrl,

                LastLoginAt = user.LastLoginAt,

                CurrentLoginAt = user.CurrentLoginAt,

                AdmissionNumber = student.AdmissionNumber,

                RollNumber = student.RollNumber,

                DepartmentName = enrollment?.Department?.Name,

                CourseName = enrollment?.Course?.Name,

                SemesterName = enrollment?.Semester.Name,

                SectionName = enrollment?.Section == null ? null: $"Section {enrollment.Section.Name}",

                AcademicSession = enrollment?.AcademicSession.Name,

                EnrollmentStatus = enrollment == null? null: (int)enrollment.EnrollmentStatus,

                EnrollmentStatusName = enrollment?.EnrollmentStatus.ToString(),

                AvatarInitials = $"{user.FirstName.FirstOrDefault()}{user.LastName.FirstOrDefault()}".ToUpper(),
            };
        }

        var teacher = await _dbContext.Teachers
                        .Include(x => x.Department)
                        .FirstOrDefaultAsync(x => x.UserId == userId);

        if (teacher != null)
        {

            return new ProfileResponse
            {
                UserId = user.Id,

                TeacherId = teacher.Id,

                InstitutionId = teacher.InstitutionId,

                CampusId = teacher.CampusId,

                InstitutionName = user.Institution.Name,

                CampusName = user.Campus.Name,

                FullName = string.Join(" ", user.FirstName, user.LastName),

                Email = user.Email,

                PhoneNumber = user.PhoneNumber,

                Role = RoleConstants.Teacher,

                IsActive = teacher.IsActive,

                EmployeeCode = teacher.EmployeeCode,

                Designation = teacher.Designation,

                ProfilePhotoUrl = user.ProfilePhotoUrl,

                LastLoginAt = user.LastLoginAt,

                CurrentLoginAt = user.CurrentLoginAt,

                AvatarInitials = $"{user.FirstName.FirstOrDefault()}{user.LastName.FirstOrDefault()}".ToUpper(),
            };
        }

        if (_scope.IsSuperAdmin())
        {
            return BuildAdminProfile(user, RoleConstants.SuperAdmin);
        }

        if (_scope.IsPlatformAdmin())
        {
            return BuildAdminProfile(user, RoleConstants.PlatformAdmin);
        }

        if (_scope.IsInstitutionAdmin())
        {
            return BuildAdminProfile(user, RoleConstants.InstitutionAdmin);
        }

        if (_scope.IsCampusAdmin())
        {
            return BuildAdminProfile(user, RoleConstants.CampusAdmin);
        }

        throw new Exception("Unable to determine user profile.");
    }

    public async Task<ProfileResponse> UpdateAsync(UpdateProfileRequest request)
    {
        var userId = _scope.UserId();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new Exception("User not found.");
        }

        user.PhoneNumber = request.PhoneNumber.Trim();

        await _dbContext.SaveChangesAsync();

        return await GetMyProfileAsync();
    }

    public Task<string> UploadPhotoAsync(IFormFile file)
    {
        throw new NotImplementedException("Profile photo upload will be implemented with the common file storage module.");
    }

    public Task RemovePhotoAsync()
    {
        throw new NotImplementedException("Profile photo removal will be implemented with the common file storage module.");
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request)
    {
        request.CurrentPassword = request.CurrentPassword.Trim();

        request.NewPassword = request.NewPassword.Trim();

        var userId = _scope.UserId();

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new Exception("User not found.");
        }

        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new Exception("Current password is incorrect.");
        }

        _passwordService.ValidatePasswordPolicy(request.NewPassword);

        if (_passwordService.VerifyPassword(request.NewPassword, user.PasswordHash))
        {
            throw new Exception("The new password must be different from the current password.");
        }

        user.PasswordHash = _passwordService.HashPassword(request.NewPassword);

        await _dbContext.SaveChangesAsync();
    }

    private static ProfileResponse BuildAdminProfile(User user, string role)
    {
        return new ProfileResponse
        {
            UserId = user.Id,

            InstitutionId = user.InstitutionId,

            CampusId = user.CampusId,

            InstitutionName = user.Institution?.Name ?? string.Empty,

            CampusName = user.Campus?.Name ?? string.Empty,

            FullName = string.Join(" ", user.FirstName, user.LastName),

            Email = user.Email,

            PhoneNumber = user.PhoneNumber,

            Role = role,

            IsActive = user.IsActive,

            ProfilePhotoUrl = user.ProfilePhotoUrl,

            LastLoginAt = user.LastLoginAt,

            CurrentLoginAt = user.CurrentLoginAt,

            AvatarInitials = $"{user.FirstName.FirstOrDefault()}{user.LastName.FirstOrDefault()}".ToUpper(),
        };
    }
}