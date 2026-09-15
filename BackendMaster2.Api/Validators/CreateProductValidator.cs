using FluentValidation;
using BackendMaster2.Api.Models;

namespace BackendMaster2.Api.Validators;
using BackendMaster2.Api.Models;
public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {

    }
}
