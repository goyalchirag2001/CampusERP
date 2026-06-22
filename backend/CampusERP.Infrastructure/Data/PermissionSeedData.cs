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
        }
    ];
}