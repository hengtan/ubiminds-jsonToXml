using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Ubiminds.Application.Commands.ConvertJsonFileToXML;

public class ConvertJsonFileToXmlValidator : AbstractValidator<ConvertJsonFileToXmlCommand>
{
    public ConvertJsonFileToXmlValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("A file is required.")
            .Must(BeAJsonFile).WithMessage("Only .json files are supported.")
            .Must(f => f.Length > 0).WithMessage("Uploaded file is empty.");
    }

    private static bool BeAJsonFile(IFormFile file)
    {
        return file.ContentType == "application/json"
               || file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
    }
}