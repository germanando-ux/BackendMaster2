using BackendMaster2.Modules.Dtos;
using FluentValidation;

namespace BackendMaster2.Api.Validators;

// 1. VALIDADOR DE COLOR
public class ColorValidator : AbstractValidator<ColorDto>
{
    public ColorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del color es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.HexCode)
            .NotEmpty().WithMessage("El código hexadecimal es obligatorio.")
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("El código hexadecimal debe tener el formato #RRGGBB (ejemplo: #E53935).");
    }
}

// 2. VALIDADOR DE CATEGORÍA
public class CategoryValidator : AbstractValidator<CategoryDto>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        // IsActive es bool, no puede ser null por defecto en C#, pero si fuera bool? sería necesario.
        // Lo dejamos por si acaso tu DTO lo tiene como nullable.
        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("El campo IsActive es obligatorio.");
    }
}

// 3. VALIDADOR DE GRUPO DE TALLAS
public class SizeGroupValidator : AbstractValidator<SizeGroupDto>
{
    public SizeGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del grupo de tallas es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");
    }
}

// 4. VALIDADOR DE TALLA
public class SizeValidator : AbstractValidator<SizeDto>
{
    public SizeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la talla es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");

        // ¡IMPORTANTE! Validar la clave foránea
        RuleFor(x => x.SizeGroupId)
            .NotEmpty().WithMessage("El Id del grupo de tallas es obligatorio.");
    }
}

// 5. VALIDADOR DE CARACTERÍSTICA (FEATURE)
public class FeatureValidator : AbstractValidator<FeatureDto>
{
    public FeatureValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la característica es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.DataType)
            .IsInEnum().WithMessage("El tipo de dato debe ser un valor válido (Text, List, Number, Boolean).");

        RuleFor(x => x.IsRequired)
            .NotNull().WithMessage("El campo IsRequired es obligatorio.");
    }
}

// 6. VALIDADOR DE OPCIÓN DE CARACTERÍSTICA (FEATURE OPTION)
public class FeatureOptionValidator : AbstractValidator<FeatureOptionDto>
{
    public FeatureOptionValidator()
    {
        RuleFor(x => x.FeatureId)
            .NotEmpty().WithMessage("El Id de la característica padre es obligatorio.");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("El valor de la opción es obligatorio.")
            .MaximumLength(100).WithMessage("El valor no puede superar 100 caracteres.");
    }
}