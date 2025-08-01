using System.Reflection;
using FluentValidation;
using MediatR;
using Ubiminds.Application.Commands.ConvertToXml;
using Ubiminds.Application.Common.Behaviors;
using Ubiminds.Application.Common.Validation;

namespace Ubiminds.Api.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ConvertToXmlCommandHandler).Assembly));

        services.AddValidatorsFromAssemblyContaining<ConvertToXmlValidator>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}