using Microsoft.OpenApi.Models;

namespace Ubiminds.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "JSON to XML Converter API",
                Version = "v1",
                Description = "API to validate and convert JSON into XML files using a background queue."
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "JSON to XML Converter API v1");
            options.RoutePrefix = "docs";
        });

        return app;
    }
}