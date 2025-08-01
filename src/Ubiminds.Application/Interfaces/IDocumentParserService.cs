using Microsoft.AspNetCore.Http;
using Ubiminds.Application.Common;
using Ubiminds.Domain.Models.InputModels;

namespace Ubiminds.Application.Interfaces;

public interface IDocumentParserService
{
    Task<Result<DocumentInputModel>> ParseAsync(IFormFile file, CancellationToken cancellationToken);
}