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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ConfigureGlobalFilters(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            
            if (entry.Entity is not BaseEntity entity)
            {
                continue;
            }

            if (entry.State == EntityState.Deleted)
            {
                entity.IsDeleted = true;

                entity.UpdatedAt = DateTime.UtcNow;

                entity.UpdatedBy =
                    _currentUserService.Email ??
                    "System";

                entry.State = EntityState.Modified;

                continue;
            }

            var userEmail =
                _currentUserService.Email ??
                "System";

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

        return await base.SaveChangesAsync(
            cancellationToken);
    }

    private void ConfigureGlobalFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Teacher>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Course>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Department>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<User>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Campus>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Institution>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Semester>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Subject>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<SemesterSubject>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<TeacherAssignment>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Role>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Permission>()
            .HasQueryFilter(x => !x.IsDeleted);
    }
}