using MediatR;
using Ubiminds.Application.Commands.ConvertToXml;
using Ubiminds.Application.Common;
using Ubiminds.Application.Interfaces;

namespace Ubiminds.Application.Commands.ConvertJsonFileToXML;

public class ConvertJsonFileToXmlCommandHandler(
    IMediator mediator,
    IDocumentParserService parser)
    : IRequestHandler<ConvertJsonFileToXmlCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConvertJsonFileToXmlCommand request, CancellationToken cancellationToken)
    {
        var parseResult = await parser.ParseAsync(request.File, cancellationToken);

        if (parseResult.IsFailure)
            return Result<string>.Failure(parseResult.Error);

        return await mediator.Send(new ConvertToXmlCommand(parseResult.Value ?? throw new InvalidOperationException()), cancellationToken);
    }
}