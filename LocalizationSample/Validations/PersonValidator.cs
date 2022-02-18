using FluentValidation;
using LocalizationSample.Controllers;
using LocalizationSample.Resources;

namespace LocalizationSample.Validations;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(p => p.FirstName).NotEmpty().WithMessage(Messages.FieldRequired).WithName(Messages.FirstName);
        RuleFor(p => p.LastName).NotEmpty().WithMessage(Messages.FieldRequired).WithName(Messages.LastName);
    }
}
