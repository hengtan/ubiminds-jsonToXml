using System.Reflection;
using FluentValidation;

namespace Ubiminds.Api.Extensions;


public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.Load("Ubiminds.Application")));

        services.AddValidatorsFromAssembly(Assembly.Load("Ubiminds.Application"));

        return services;
    }
}