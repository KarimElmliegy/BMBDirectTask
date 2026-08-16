using BMBAssessment.Application.Mappings;
using BMBAssessment.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BMBAssessment.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
