using Microsoft.OpenApi;

namespace Shop.Api.Extensions;

public static class SwaggerServiceCollectionExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Shop.Api",
                Version = "v1"
            });
        });

        return services;
    }
}
