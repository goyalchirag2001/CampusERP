using CampusERP.Application.Interfaces;
using CampusERP.Infrastructure.Authentication;
using CampusERP.Infrastructure.Data;
using CampusERP.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CampusERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IPasswordService, PasswordService>();

        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }
}