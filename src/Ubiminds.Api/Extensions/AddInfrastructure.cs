using FluentValidation;
using Ubiminds.Application.Commands.ConvertToXml;
using Ubiminds.Domain.Interfaces;
using Ubiminds.Infrastructure;
using Ubiminds.Infrastructure.Messaging.InMemory;
using Ubiminds.Infrastructure.Xml.Interfaces;

namespace Ubiminds.Api.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryQueue>();
        services.AddSingleton<IXmlConverter, XmlConverter>();
        services.AddScoped<IMessagePublisher, InMemoryPublisher>();
        services.AddHostedService<InMemoryBackgroundConsumer>();
        services.AddValidatorsFromAssemblyContaining<ConvertToXmlValidator>();

        return services;
    }
}