using FluentValidation;
using Ubiminds.Application.Common.Validation;
using Ubiminds.Application.Interfaces;
using Ubiminds.Application.Services;
using Ubiminds.Domain.Interfaces;
using Ubiminds.Infrastructure.Interfaces;
using Ubiminds.Infrastructure.Messaging.InMemory;
using Ubiminds.Infrastructure.Services;

namespace Ubiminds.Api.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryQueue>();
        services.AddScoped<IMessagePublisher, InMemoryPublisher>();
        services.AddScoped<IDocumentParserService, DocumentParserService>();
        services.AddHostedService<InMemoryBackgroundConsumer>();
        services.AddValidatorsFromAssemblyContaining<ConvertToXmlValidator>();
        services.AddSingleton<IXmlConverterService, XmlConverterService>();

        return services;
    }
}