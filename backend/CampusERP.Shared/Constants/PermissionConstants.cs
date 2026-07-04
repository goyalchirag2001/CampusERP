namespace CampusERP.Shared.Constants;

public static class PermissionConstants
{
    // Dashboard

    public const string AdminDashboardView = "AdminDashboard.View";

    // Institution

    public const string InstitutionView = "Institution.View";
    public const string InstitutionCreate = "Institution.Create";
    public const string InstitutionEdit = "Institution.Edit";
    public const string InstitutionActivate = "Institution.Activate";
    public const string InstitutionDeactivate = "Institution.Deactivate";

    // Campus

    public const string CampusView = "Campus.View";
    public const string CampusCreate = "Campus.Create";
    public const string CampusEdit = "Campus.Edit";
    public const string CampusActivate = "Campus.Activate";
    public const string CampusDeactivate = "Campus.Deactivate";

    // Department

    public const string DepartmentView = "Department.View";
    public const string DepartmentCreate = "Department.Create";
    public const string DepartmentEdit = "Department.Edit";
    public const string DepartmentActivate = "Department.Activate";
    public const string DepartmentDeactivate = "Department.Deactivate";

    // Course

    public const string CourseView = "Course.View";
    public const string CourseCreate = "Course.Create";
    public const string CourseEdit = "Course.Edit";
    public const string CourseActivate = "Course.Activate";
    public const string CourseDeactivate = "Course.Deactivate";

    // Semester

    public const string SemesterView = "Semester.View";
    public const string SemesterCreate = "Semester.Create";
    public const string SemesterEdit = "Semester.Edit";
    public const string SemesterActivate = "Semester.Activate";
    public const string SemesterDeactivate = "Semester.Deactivate";

    // Subject

    public const string SubjectView = "Subject.View";
    public const string SubjectCreate = "Subject.Create";
    public const string SubjectEdit = "Subject.Edit";
    public const string SubjectActivate = "Subject.Activate";
    public const string SubjectDeactivate = "Subject.Deactivate";

    // Teacher

    public const string TeacherView = "Teacher.View";
    public const string TeacherCreate = "Teacher.Create";
    public const string TeacherEdit = "Teacher.Edit";
    public const string TeacherActivate = "Teacher.Activate";
    public const string TeacherDeactivate = "Teacher.Deactivate";

    // Student

    public const string StudentView = "Student.View";
    public const string StudentCreate = "Student.Create";
    public const string StudentEdit = "Student.Edit";
    public const string StudentActivate = "Student.Activate";
    public const string StudentDeactivate = "Student.Deactivate";

    // User

    public const string UserView = "User.View";
    public const string UserCreate = "User.Create";
    public const string UserEdit = "User.Edit";
    public const string UserActivate = "User.Activate";
    public const string UserDeactivate = "User.Deactivate";
    public const string UserResetPassword = "User.ResetPassword";

    // Role

    public const string RoleView = "Role.View";
    public const string RoleCreate = "Role.Create";
    public const string RoleEdit = "Role.Edit";
    public const string RoleActivate = "Role.Activate";
    public const string RoleDeactivate = "Role.Deactivate";

    // Teacher Assignment

    public const string TeacherAssignmentView = "TeacherAssignment.View";
    public const string TeacherAssignmentCreate = "TeacherAssignment.Create";
    public const string TeacherAssignmentDelete = "TeacherAssignment.Delete";

    // Semester Subject Assignment

    public const string SemesterSubjectView = "SemesterSubject.View";
    public const string SemesterSubjectAssign = "SemesterSubject.Assign";
    public const string SemesterSubjectRemove = "SemesterSubject.Remove";

    // Section

    public const string SectionView = "Section.View";
    public const string SectionCreate = "Section.Create";
    public const string SectionEdit = "Section.Edit";
    public const string SectionActivate = "Section.Activate";
    public const string SectionDeactivate = "Section.Deactivate";

    // Academic Session

    public const string AcademicSessionView = "Academic-Session.View";
    public const string AcademicSessionCreate = "Academic-Session.Create";
    public const string AcademicSessionEdit = "Academic-Session.Edit";
    public const string AcademicSessionActivate = "Academic-Session.Activate";
    public const string AcademicSessionDeactivate = "Academic-Session.Deactivate";

    public static readonly List<string> All =
    [
        AdminDashboardView,

        InstitutionView,
        InstitutionCreate,
        InstitutionEdit,
        InstitutionActivate,
        InstitutionDeactivate,

        CampusView,
        CampusCreate,
        CampusEdit,
        CampusActivate,
        CampusDeactivate,

        DepartmentView,
        DepartmentCreate,
        DepartmentEdit,
        DepartmentActivate,
        DepartmentDeactivate,

        CourseView,
        CourseCreate,
        CourseEdit,
        CourseActivate,
        CourseDeactivate,

        SemesterView,
        SemesterCreate,
        SemesterEdit,
        SemesterActivate,
        SemesterDeactivate,

        SubjectView,
        SubjectCreate,
        SubjectEdit,
        SubjectActivate,
        SubjectDeactivate,

        TeacherView,
        TeacherCreate,
        TeacherEdit,
        TeacherActivate,
        TeacherDeactivate,

        StudentView,
        StudentCreate,
        StudentEdit,
        StudentActivate,
        StudentDeactivate,

        UserView,
        UserCreate,
        UserEdit,
        UserActivate,
        UserDeactivate,
        UserResetPassword,

        RoleView,
        RoleCreate,
        RoleEdit,
        RoleActivate,
        RoleDeactivate,

        TeacherAssignmentView,
        TeacherAssignmentCreate,
        TeacherAssignmentDelete,

        SemesterSubjectView,
        SemesterSubjectAssign,
        SemesterSubjectRemove,

        SectionView,
        SectionCreate,
        SectionEdit,
        SectionActivate,
        SectionDeactivate,

        AcademicSessionView,
        AcademicSessionCreate,
        AcademicSessionEdit,
        AcademicSessionActivate,
        AcademicSessionDeactivate
    ];
}
