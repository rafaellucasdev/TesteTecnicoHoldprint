using ContasAPagar.API.DTOs.Payable;
using FluentValidation;

namespace ContasAPagar.API.Validators;

public class CreatePayableRequestValidator : AbstractValidator<CreatePayableRequest>
{
    public CreatePayableRequestValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("ID do fornecedor é obrigatório");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição é obrigatória")
            .MaximumLength(500).WithMessage("Descrição não pode ter mais de 500 caracteres");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Data de vencimento é obrigatória");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero");
    }
}

