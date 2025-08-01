using MediatR;
using Ubiminds.Application.Common;
using Ubiminds.Domain.Interfaces;
using Ubiminds.Infrastructure.Interfaces;

namespace Ubiminds.Application.Commands.ConvertToXml;

public class ConvertToXmlCommandHandler(IMessagePublisher bus, IXmlConverterService converter)
    : IRequestHandler<ConvertToXmlCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConvertToXmlCommand request, CancellationToken cancellationToken)
    {
        var xml = converter.ConvertToXml(request.Data);

        await bus.PublishAsync("convert-xml-queue", request.Data);

        return Result<string>.Success("Scheduled for conversion");
    }
}