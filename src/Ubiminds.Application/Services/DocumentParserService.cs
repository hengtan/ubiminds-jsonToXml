using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Ubiminds.Application.Common;
using Ubiminds.Application.Interfaces;
using Ubiminds.Domain.Models.InputModels;

namespace Ubiminds.Application.Services;


public class DocumentParserService : IDocumentParserService
{
    public async Task<Result<DocumentInputModel>> ParseAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (IsFileInvalid(file))
            return Result<DocumentInputModel>.Failure("File is null or empty.");

        try
        {
            var content = await ReadFileContentAsync(file, cancellationToken);

            var model = DeserializeContent(content);

            return model is null
                ? Result<DocumentInputModel>.Failure("Invalid JSON content.")
                : Result<DocumentInputModel>.Success(model);
        }
        catch (JsonException ex)
        {
            return Result<DocumentInputModel>.Failure($"JSON parsing failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<DocumentInputModel>.Failure($"Unexpected error: {ex.Message}");
        }
    }

    private static bool IsFileInvalid(IFormFile file)
    {
        return file is null || file.Length == 0;
    }

    private static async Task<string> ReadFileContentAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        using var reader = new StreamReader(memoryStream);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private static DocumentInputModel? DeserializeContent(string content)
    {
        return JsonSerializer.Deserialize<DocumentInputModel>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}