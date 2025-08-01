using MediatR;
using Microsoft.AspNetCore.Http;
using Ubiminds.Application.Common;

namespace Ubiminds.Application.Commands.ConvertJsonFileToXML;

public record ConvertJsonFileToXmlCommand(IFormFile File) : IRequest<Result<string>>;