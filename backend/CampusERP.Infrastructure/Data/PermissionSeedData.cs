using CampusERP.Domain.Entities;
using CampusERP.Shared.Constants;

namespace CampusERP.Infrastructure.Data;

public static class PermissionSeedData
{
    public static readonly Guid AdminDasboardViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000001");

    public static readonly Guid InstitutionViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid InstitutionCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    public static readonly Guid InstitutionEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000004");
    public static readonly Guid InstitutionActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000005");
    public static readonly Guid InstitutionDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000006");

    public static readonly Guid CampusViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000007");
    public static readonly Guid CampusCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000008");
    public static readonly Guid CampusEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000009");
    public static readonly Guid CampusActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000010");
    public static readonly Guid CampusDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000011");

    public static readonly Guid DepartmentViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000012");
    public static readonly Guid DepartmentCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000013");
    public static readonly Guid DepartmentEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000014");
    public static readonly Guid DepartmentActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000015");
    public static readonly Guid DepartmentDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000016");

    public static readonly Guid UserViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000017");
    public static readonly Guid UserCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000018");
    public static readonly Guid UserEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000019");
    public static readonly Guid UserActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000020");
    public static readonly Guid UserDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000021");

    public static readonly Guid RoleViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000022");
    public static readonly Guid RoleCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000023");
    public static readonly Guid RoleEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000024");
    public static readonly Guid RoleActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000025");
    public static readonly Guid RoleDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000026");

    public static readonly Guid TeacherAssignmentViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000027");
    public static readonly Guid TeacherAssignmentCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000028");
    public static readonly Guid TeacherAssignmentDeletePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000029");

    public static readonly Guid SemesterSubjectViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000030");
    public static readonly Guid SemesterSubjectAssignPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000031");
    public static readonly Guid SemesterSubjectRemovePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000032");

    public static readonly Guid CourseViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000033");
    public static readonly Guid CourseCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000034");
    public static readonly Guid CourseEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000035");
    public static readonly Guid CourseActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000036");
    public static readonly Guid CourseDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000037");

    public static readonly Guid SemesterViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000038");
    public static readonly Guid SemesterCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000039");
    public static readonly Guid SemesterEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000040");
    public static readonly Guid SemesterActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000041");
    public static readonly Guid SemesterDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000042");

    public static readonly Guid SubjectViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000043");
    public static readonly Guid SubjectCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000044");
    public static readonly Guid SubjectEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000045");
    public static readonly Guid SubjectActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000046");
    public static readonly Guid SubjectDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000047");

    public static readonly Guid TeacherViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000048");
    public static readonly Guid TeacherCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000049");
    public static readonly Guid TeacherEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000050");
    public static readonly Guid TeacherActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000051");
    public static readonly Guid TeacherDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000052");

    public static readonly Guid StudentViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000053");
    public static readonly Guid StudentCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000054");
    public static readonly Guid StudentEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000055");
    public static readonly Guid StudentActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000056");
    public static readonly Guid StudentDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000057");

    public static readonly Guid UserResetPasswordPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000058");

    public static readonly Guid SectionViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000059");
    public static readonly Guid SectionCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000060");
    public static readonly Guid SectionEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000061");
    public static readonly Guid SectionActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000062");
    public static readonly Guid SectionDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000063");

    public static readonly Guid AcademicSessionViewPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000064");
    public static readonly Guid AcademicSessionCreatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000065");
    public static readonly Guid AcademicSessionEditPermissionId = Guid.Parse("10000000-0000-0000-0000-000000000066");
    public static readonly Guid AcademicSessionActivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000067");
    public static readonly Guid AcademicSessionDeactivatePermissionId = Guid.Parse("10000000-0000-0000-0000-000000000068");

    public static readonly List<Permission> Permissions =
    [
        // Admin Dashboard
        new()
        {
            Id = AdminDasboardViewPermissionId,
            Code = PermissionConstants.AdminDashboardView,
            Name = "View Admin Dashboard",
            Module = "AdminDashboard"
        },

        // Institution

        new()
        {
            Id = InstitutionViewPermissionId,
            Code = PermissionConstants.InstitutionView,
            Name = "View Institutions",
            Module = "Institution"
        },

        new()
        {
            Id = InstitutionCreatePermissionId,
            Code = PermissionConstants.InstitutionCreate,
            Name = "Create Institutions",
            Module = "Institution"
        },

        new()
        {
            Id = InstitutionEditPermissionId,
            Code = PermissionConstants.InstitutionEdit,
            Name = "Edit Institutions",
            Module = "Institution"
        },

        new()
        {
            Id = InstitutionActivatePermissionId,
            Code = PermissionConstants.InstitutionActivate,
            Name = "Activate Institutions",
            Module = "Institution"
        },

        new()
        {
            Id = InstitutionDeactivatePermissionId,
            Code = PermissionConstants.InstitutionDeactivate,
            Name = "Deactivate Institutions",
            Module = "Institution"
        },

        // Campus

        new()
        {
            Id = CampusViewPermissionId,
            Code = PermissionConstants.CampusView,
            Name = "View Campuses",
            Module = "Campus"
        },

        new()
        {
            Id = CampusCreatePermissionId,
            Code = PermissionConstants.CampusCreate,
            Name = "Create Campuses",
            Module = "Campus"
        },

        new()
        {
            Id = CampusEditPermissionId,
            Code = PermissionConstants.CampusEdit,
            Name = "Edit Campuses",
            Module = "Campus"
        },

        new()
        {
            Id = CampusActivatePermissionId,
            Code = PermissionConstants.CampusActivate,
            Name = "Activate Campuses",
            Module = "Campus"
        },

        new()
        {
            Id = CampusDeactivatePermissionId,
            Code = PermissionConstants.CampusDeactivate,
            Name = "Deactivate Campuses",
            Module = "Campus"
        },

        // Department

        new()
        {
            Id = DepartmentViewPermissionId,
            Code = PermissionConstants.DepartmentView,
            Name = "View Departments",
            Module = "Department"
        },

        new()
        {
            Id = DepartmentCreatePermissionId,
            Code = PermissionConstants.DepartmentCreate,
            Name = "Create Departments",
            Module = "Department"
        },

        new()
        {
            Id = DepartmentEditPermissionId,
            Code = PermissionConstants.DepartmentEdit,
            Name = "Edit Departments",
            Module = "Department"
        },

        new()
        {
            Id = DepartmentActivatePermissionId,
            Code = PermissionConstants.DepartmentActivate,
            Name = "Activate Departments",
            Module = "Department"
        },

        new()
        {
            Id = DepartmentDeactivatePermissionId,
            Code = PermissionConstants.DepartmentDeactivate,
            Name = "Deactivate Departments",
            Module = "Department"
        },

        // Course
        new()
        {
            Id = CourseViewPermissionId,
            Code = PermissionConstants.CourseView,
            Name = "View Courses",
            Module = "Course"
        },

        new()
        {
            Id = CourseCreatePermissionId,
            Code = PermissionConstants.CourseCreate,
            Name = "Create Courses",
            Module = "Course"
        },

        new()
        {
            Id = CourseEditPermissionId,
            Code = PermissionConstants.CourseEdit,
            Name = "Edit Courses",
            Module = "Course"
        },

        new()
        {
            Id = CourseActivatePermissionId,
            Code = PermissionConstants.CourseActivate,
            Name = "Activate Courses",
            Module = "Course"
        },

        new()
        {
            Id = CourseDeactivatePermissionId,
            Code = PermissionConstants.CourseDeactivate,
            Name = "Deactivate Courses",
            Module = "Course"
        },

        // Semester
        new()
        {
            Id = SemesterViewPermissionId,
            Code = PermissionConstants.SemesterView,
            Name = "View Semesters",
            Module = "Semester"
        },

        new()
        {
            Id = SemesterCreatePermissionId,
            Code = PermissionConstants.SemesterCreate,
            Name = "Create Semesters",
            Module = "Semester"
        },

        new()
        {
            Id = SemesterEditPermissionId,
            Code = PermissionConstants.SemesterEdit,
            Name = "Edit Semesters",
            Module = "Semester"
        },

        new()
        {
            Id = SemesterActivatePermissionId,
            Code = PermissionConstants.SemesterActivate,
            Name = "Activate Semesters",
            Module = "Semester"
        },

        new()
        {
            Id = SemesterDeactivatePermissionId,
            Code = PermissionConstants.SemesterDeactivate,
            Name = "Deactivate Semesters",
            Module = "Semester"
        },

        // Subject
        new()
        {
            Id = SubjectViewPermissionId,
            Code = PermissionConstants.SubjectView,
            Name = "View Subjects",
            Module = "Subject"
        },

        new()
        {
            Id = SubjectCreatePermissionId,
            Code = PermissionConstants.SubjectCreate,
            Name = "Create Subjects",
            Module = "Subject"
        },

        new()
        {
            Id =SubjectEditPermissionId,
            Code = PermissionConstants.SubjectEdit,
            Name = "Edit Subjects",
            Module = "Subject"
        },

        new()
        {
            Id = SubjectActivatePermissionId,
            Code = PermissionConstants.SubjectActivate,
            Name = "Activate Subjects",
            Module = "Subject"
        },

        new()
        {
            Id = SubjectDeactivatePermissionId,
            Code = PermissionConstants.SubjectDeactivate,
            Name = "Deactivate Subjects",
            Module = "Subject"
        },

        // Teacher
        new()
        {
            Id = TeacherViewPermissionId,
            Code = PermissionConstants.TeacherView,
            Name = "View Teachers",
            Module = "Teacher"
        },

        new()
        {
            Id = TeacherCreatePermissionId,
            Code = PermissionConstants.TeacherCreate,
            Name = "Create Teachers",
            Module = "Teacher"
        },

        new()
        {
            Id = TeacherEditPermissionId,
            Code = PermissionConstants.TeacherEdit,
            Name = "Edit Teachers",
            Module = "Teacher"
        },

        new()
        {
            Id = TeacherActivatePermissionId,
            Code = PermissionConstants.TeacherActivate,
            Name = "Activate Teachers",
            Module = "Teacher"
        },

        new()
        {
            Id = TeacherDeactivatePermissionId,
            Code = PermissionConstants.TeacherDeactivate,
            Name = "Deactivate Teachers",
            Module = "Teacher"
        },

        // Student
        new()
        {
            Id = StudentViewPermissionId,
            Code = PermissionConstants.StudentView,
            Name = "View Students",
            Module = "Student"
        },

        new()
        {
            Id = StudentCreatePermissionId,
            Code = PermissionConstants.StudentCreate,
            Name = "Create Students",
            Module = "Student"
        },

        new()
        {
            Id = StudentEditPermissionId,
            Code = PermissionConstants.StudentEdit,
            Name = "Edit Students",
            Module = "Student"
        },

        new()
        {
            Id = StudentActivatePermissionId,
            Code = PermissionConstants.StudentActivate,
            Name = "Activate Students",
            Module = "Student"
        },

        new()
        {
            Id = StudentDeactivatePermissionId,
            Code = PermissionConstants.StudentDeactivate,
            Name = "Deactivate Students",
            Module = "Student"
        },

        // Reset Password

        new()
        {
            Id = UserResetPasswordPermissionId,
            Code = PermissionConstants.UserResetPassword,
            Name = "Reset User Password",
            Module = "User"
        },


        // User

        new()
        {
            Id = UserViewPermissionId,
            Code = PermissionConstants.UserView,
            Name = "View Users",
            Module = "User"
        },

        new()
        {
            Id = UserCreatePermissionId,
            Code = PermissionConstants.UserCreate,
            Name = "Create Users",
            Module = "User"
        },

        new()
        {
            Id = UserEditPermissionId,
            Code = PermissionConstants.UserEdit,
            Name = "Edit Users",
            Module = "User"
        },

        new()
        {
            Id = UserActivatePermissionId,
            Code = PermissionConstants.UserActivate,
            Name = "Activate Users",
            Module = "User"
        },

        new()
        {
            Id = UserDeactivatePermissionId,
            Code = PermissionConstants.UserDeactivate,
            Name = "Deactivate Users",
            Module = "User"
        },

        // Role

        new()
        {
            Id = RoleViewPermissionId,
            Code = PermissionConstants.RoleView,
            Name = "View Roles",
            Module = "Role"
        },

        new()
        {
            Id = RoleCreatePermissionId,
            Code = PermissionConstants.RoleCreate,
            Name = "Create Roles",
            Module = "Role"
        },

        new()
        {
            Id = RoleEditPermissionId,
            Code = PermissionConstants.RoleEdit,
            Name = "Edit Roles",
            Module = "Role"
        },

        new()
        {
            Id = RoleActivatePermissionId,
            Code = PermissionConstants.RoleActivate,
            Name = "Activate Roles",
            Module = "Role"
        },

        new()
        {
            Id = RoleDeactivatePermissionId,
            Code = PermissionConstants.RoleDeactivate,
            Name = "Deactivate Roles",
            Module = "Role"
        },

        // Teacher Assignment

        new()
        {
            Id = TeacherAssignmentViewPermissionId,
            Code = PermissionConstants.TeacherAssignmentView,
            Name = "View Teacher Assignments",
            Module = "TeacherAssignment"
        },

        new()
        {
            Id = TeacherAssignmentCreatePermissionId,
            Code = PermissionConstants.TeacherAssignmentCreate,
            Name = "Create Teacher Assignments",
            Module = "TeacherAssignment"
        },

        new()
        {
            Id = TeacherAssignmentDeletePermissionId,
            Code = PermissionConstants.TeacherAssignmentDelete,
            Name = "Delete Teacher Assignments",
            Module = "TeacherAssignment"
        },

        // Semester Subject Assignment

        new()
        {
            Id = SemesterSubjectViewPermissionId,
            Code = PermissionConstants.SemesterSubjectView,
            Name = "View Semester Subject Assignments",
            Module = "SemesterSubject"
        },

        new()
        {
            Id = SemesterSubjectAssignPermissionId,
            Code = PermissionConstants.SemesterSubjectAssign,
            Name = "Assign Semester Subjects",
            Module = "SemesterSubject"
        },

        new()
        { 
            Id = SemesterSubjectRemovePermissionId,
            Code = PermissionConstants.SemesterSubjectRemove,
            Name = "Remove Semester Subjects",
            Module = "SemesterSubject"
        },

        // Section

        new()
        {
            Id = SectionViewPermissionId,
            Code = PermissionConstants.SectionView,
            Name = "View Sections",
            Module = "Section"
        },

        new()
        {
            Id = SectionCreatePermissionId,
            Code = PermissionConstants.SectionCreate,
            Name = "Create Sections",
            Module = "Section"
        },

        new()
        {
            Id = SectionEditPermissionId,
            Code = PermissionConstants.SectionEdit,
            Name = "Edit Sections",
            Module = "Section"
        },

        new()
        {
            Id = SectionActivatePermissionId,
            Code = PermissionConstants.SectionActivate,
            Name = "Activate Sections",
            Module = "Section"
        },

        new()
        {
            Id = SectionDeactivatePermissionId,
            Code = PermissionConstants.SectionDeactivate,
            Name = "Deactivate Sections",
            Module = "Section"
        },

        // Academic Session

        new()
        {
            Id = AcademicSessionViewPermissionId,
            Code = PermissionConstants.AcademicSessionView,
            Name = "View Academic Sessions",
            Module = "AcademicSession"
        },
        new()
        {
            Id = AcademicSessionCreatePermissionId,
            Code = PermissionConstants.AcademicSessionCreate,
            Name = "Create Academic Sessions",
            Module = "AcademicSession"
        },
        new()
        {
            Id = AcademicSessionEditPermissionId,
            Code = PermissionConstants.AcademicSessionEdit,
            Name = "Edit Academic Sessions",
            Module = "AcademicSession"
        },
        new()
        {
            Id = AcademicSessionActivatePermissionId,
            Code = PermissionConstants.AcademicSessionActivate,
            Name = "Activate Academic Sessions",
            Module = "AcademicSession"
        },
        new()
        {
            Id = AcademicSessionDeactivatePermissionId,
            Code = PermissionConstants.AcademicSessionDeactivate,
            Name = "Deactivate Academic Sessions",
            Module = "AcademicSession"
        }
    ];
}