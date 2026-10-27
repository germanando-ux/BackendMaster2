using FluentValidation;
using BackendMaster2.Modules.Dtos; // Ajusta al namespace real de tus DTOs

namespace BackendMaster2.Api.Validators;

public class MarketplaceValidator : AbstractValidator<MarketplaceDto>
{
    public MarketplaceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es obligatorio.")
            .MaximumLength(50).WithMessage("Máximo 50 caracteres.");
    }
}

public class DataSourceValidator : AbstractValidator<DataSourceDto>
{
    public DataSourceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es obligatorio.")
            .MaximumLength(50).WithMessage("Máximo 50 caracteres.");
    }
}