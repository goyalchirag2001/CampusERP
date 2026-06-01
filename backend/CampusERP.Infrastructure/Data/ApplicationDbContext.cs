using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CampusERP.Application.Interfaces;
using CampusERP.Domain.Common;

namespace CampusERP.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Institution> Institutions => Set<Institution>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Teacher> Teachers => Set<Teacher>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
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
}