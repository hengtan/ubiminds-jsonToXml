using System.Reflection;
using FluentValidation;
using MediatR;
using Ubiminds.Application.Commands.ConvertToXml;
using Ubiminds.Application.Common.Behaviors;

namespace Ubiminds.Api.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra handlers do MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ConvertToXmlCommandHandler).Assembly));

        // Registra todos os validators (baseado no tipo)
        services.AddValidatorsFromAssemblyContaining<ConvertToXmlValidator>();

        // Adiciona pipeline para validação automática
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}