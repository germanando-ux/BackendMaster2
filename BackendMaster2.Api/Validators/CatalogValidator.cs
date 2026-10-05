using BackendMaster2.Modules.Dtos;
using FluentValidation;

namespace BackendMaster2.Api.Validators;

public class CreateColorValidator : AbstractValidator<ColorDto>
{
    public CreateColorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del color es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.HexCode)
            .NotEmpty().WithMessage("El código hexadecimal es obligatorio.")
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("El código hexadecimal debe tener el formato #RRGGBB (ejemplo: #E53935)."); 
            
    }

    public class UpdateColorValidator : CreateColorValidator
    {
        public UpdateColorValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El Id es obligatorio en una actualización.");
        }
    }
}
