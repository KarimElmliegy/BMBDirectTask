using BMBAssessment.Application.Interfaces;
using BMBAssessment.Application.Interfaces.Repositories;
using BMBAssessment.Application.Services;
using BMBAssessment.Domain.Entities;
using BMBAssessment.Infrastructure.Persistence;
using BMBAssessment.Infrastructure.Persistence.Repositories;
using BMBAssessment.Infrastructure.Security;
using BMBAssessment.Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BMBAssessment.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = null!;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        services.AddScoped<IPasswordHasher<ApplicationUser>, LegacyCompatiblePasswordHasher>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderDeletionRepository, OrderDeletionRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, IdentityAuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddHostedService<OrderPurgeBackgroundService>();
        return services;
    }
}
