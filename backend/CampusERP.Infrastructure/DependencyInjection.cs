using CampusERP.Application.Interfaces;
using CampusERP.Infrastructure.Authentication;
using CampusERP.Infrastructure.Data;
using CampusERP.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CampusERP.Infrastructure.Services;
using CampusERP.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CampusERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IDataAccessScope, DataAccessScope>();

        services.AddScoped<IPasswordService, PasswordService>();

        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IInstitutionService, InstitutionService>();

        services.AddScoped<ICampusService, CampusService>();

        services.AddScoped<IDepartmentService, DepartmentService>();

        services.AddScoped<ICourseService, CourseService>();

        services.AddScoped<ISubjectService, SubjectService>();

        services.AddScoped<ISemesterSubjectService, SemesterSubjectService>();

        services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();

        services.AddScoped<ITeacherService, TeacherService>();

        services.AddScoped<IStudentService, StudentService>();

        services.AddScoped<IDashboardService, DashboardService>();

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddScoped<ISectionService, SectionService>();

        services.AddScoped<IProfileService, ProfileService>();

        services.AddScoped<IAcademicSessionService, AcademicSessionService>();

        services.AddScoped<ISemesterService, SemesterService>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }
}