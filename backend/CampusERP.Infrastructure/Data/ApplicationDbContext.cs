using System.Linq.Expressions;
using CampusERP.Application.Interfaces;
using CampusERP.Domain.Common;
using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    private readonly Guid? _institutionId;

    private readonly Guid? _campusId;

    public Guid? CurrentInstitutionId => _institutionId;

    public Guid? CurrentCampusId => _campusId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;

        _institutionId = currentUserService.InstitutionId;

        _campusId = currentUserService.CampusId;
    }

    public DbSet<Institution> Institutions => Set<Institution>();

    public DbSet<Campus> Campuses => Set<Campus>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Teacher> Teachers => Set<Teacher>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Semester> Semesters => Set<Semester>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Subject> Subjects => Set<Subject>();

    public DbSet<SemesterSubject> SemesterSubjects => Set<SemesterSubject>();

    public DbSet<TeacherAssignment> TeacherAssignments => Set<TeacherAssignment>();

    public DbSet<AcademicSession> AcademicSessions => Set<AcademicSession>();

    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();

    public DbSet<Section> Sections => Set<Section>();

    public DbSet<AcademicConfiguration> AcademicConfigurations => Set<AcademicConfiguration>();

    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();

    public DbSet<TimetableTemplate> TimetableTemplates => Set<TimetableTemplate>();

    public DbSet<LectureOverride> LectureOverrides => Set<LectureOverride>();

    public DbSet<AttendanceCorrectionRequest> AttendanceCorrectionRequests => Set<AttendanceCorrectionRequest>();

    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    public DbSet<AttendanceSession> AttendanceSessions => Set<AttendanceSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ConfigureGlobalFilters(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ConfigureGlobalFilters(ModelBuilder modelBuilder)
    {
        ApplySoftDelete<Student>(modelBuilder);
        ApplySoftDelete<Teacher>(modelBuilder);
        ApplySoftDelete<Course>(modelBuilder);
        ApplySoftDelete<Department>(modelBuilder);
        ApplySoftDelete<User>(modelBuilder);
        ApplySoftDelete<Campus>(modelBuilder);
        ApplySoftDelete<Institution>(modelBuilder);
        ApplySoftDelete<Semester>(modelBuilder);
        ApplySoftDelete<Subject>(modelBuilder);
        ApplySoftDelete<SemesterSubject>(modelBuilder);
        ApplySoftDelete<TeacherAssignment>(modelBuilder);
        ApplySoftDelete<Role>(modelBuilder);
        ApplySoftDelete<Permission>(modelBuilder);
        ApplySoftDelete<AcademicConfiguration>(modelBuilder);
        ApplySoftDelete<AcademicSession>(modelBuilder);
        ApplySoftDelete<StudentEnrollment>(modelBuilder);
        ApplySoftDelete<Section>(modelBuilder);
        ApplySoftDelete<RefreshToken>(modelBuilder);

        ApplySoftDelete<Room>(modelBuilder);
        ApplySoftDelete<CalendarEvent>(modelBuilder);
        ApplySoftDelete<TimetableTemplate>(modelBuilder);
        ApplySoftDelete<LectureOverride>(modelBuilder);

        ApplySoftDelete<AttendanceCorrectionRequest>(modelBuilder);
        ApplySoftDelete<AttendanceRecord>(modelBuilder);
        ApplySoftDelete<AttendanceSession>(modelBuilder);
    }

    private void ApplyAuditInformation()
    {
        var userEmail = _currentUserService.Email ?? "System";

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.Entity is not BaseEntity entity)
                continue;

            if (entry.State == EntityState.Deleted)
            {
                // Junction tables should be physically deleted
                if (entry.Entity is RolePermission || entry.Entity is UserRole)
                    continue;

                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userEmail;

                entry.State = EntityState.Modified;
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.CreatedBy = userEmail;

                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userEmail;
            }

            if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userEmail;
            }
        }
    }

    private static void ApplySoftDelete<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(x => !x.IsDeleted);
    }
}