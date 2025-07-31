using MediatR;
using Ubiminds.Application.Common;
using Ubiminds.Domain.Models.InputModels;

namespace Ubiminds.Application.Commands.ConvertToXml;

public record ConvertToXmlCommand(DocumentInputModel Data) : IRequest<Result<string>>;