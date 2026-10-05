using FluentValidation;
using BackendMaster2.Shared.Domain;
using BackendMaster2.Modules.Dtos;

namespace BackendMaster2.Api.Validators;

/// <summary>
/// Validadores de los endpoints de Products.
/// Agrupados por cohesión: cambian juntos y UpdateColor reutiliza las reglas de CreateColor.
/// </summary>
public class CreateProductValidator : AbstractValidator<ProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("El SKU es obligatorio.")
            .MaximumLength(50).WithMessage("El SKU no puede exceder los 50 caracteres.")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("El SKU solo admite mayúsculas, números y guiones.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor que cero.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");
    }
}

public class UpdateProductValidator : CreateProductValidator
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id es obligatorio en una actualización.");
    }
}
