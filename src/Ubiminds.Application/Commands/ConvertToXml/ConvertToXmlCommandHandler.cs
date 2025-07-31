using MediatR;
using Ubiminds.Application.Common;
using Ubiminds.Domain.Interfaces;

namespace Ubiminds.Application.Commands.ConvertToXml;

public class ConvertToXmlCommandHandler(IMessagePublisher bus) : IRequestHandler<ConvertToXmlCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConvertToXmlCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        if (data.Status != 3)
            return Result<string>.Failure("Status must be 3");

        if (data.PublishDate < new DateTime(2024, 08, 24))
            return Result<string>.Failure("PublishDate must be >= 2024-08-24");

        if (!data.TestRun)
            return Result<string>.Failure("Must be a test run");

        await bus.PublishAsync("convert-xml-queue", data);
        return Result<string>.Success("Scheduled for conversion");
    }
}