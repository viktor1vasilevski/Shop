using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Interfaces;
using Shop.Application.Services;

namespace Shop.Infrastructure.IoC;

public static class DependencyContainer
{
    public static IServiceCollection AddIoCServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
