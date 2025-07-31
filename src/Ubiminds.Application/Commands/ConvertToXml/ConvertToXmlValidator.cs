using FluentValidation;

namespace Ubiminds.Application.Commands.ConvertToXml;

public class ConvertToXmlValidator : AbstractValidator<ConvertToXmlCommand>
{
    public ConvertToXmlValidator()
    {
        RuleFor(x => x.Data)
            .NotNull().WithMessage("Data is required.");

        When(x => x.Data != null, () =>
        {
            RuleFor(x => x.Data.Status)
                .Equal(3).WithMessage("Status must be 3.");

            RuleFor(x => x.Data.TestRun)
                .Equal(true).WithMessage("TestRun must be true.");

            RuleFor(x => x.Data.PublishDate)
                .GreaterThanOrEqualTo(new DateTime(2024, 8, 24))
                .WithMessage("PublishDate must be on or after 2024-08-24.");
        });
    }
}