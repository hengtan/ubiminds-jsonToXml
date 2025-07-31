using MediatR;
using Ubiminds.Application.Common;
using Ubiminds.Domain.Interfaces;

namespace Ubiminds.Application.Commands.ConvertToXml;

public class ConvertToXmlCommandHandler(IMessagePublisher bus)
    : IRequestHandler<ConvertToXmlCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConvertToXmlCommand request, CancellationToken cancellationToken)
    {
        await bus.PublishAsync("convert-xml-queue", request.Data);

        return Result<string>.Success("Scheduled for conversion");
    }
}