using ContasAPagar.API.DTOs.Supplier;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ContasAPagar.API.Validators;

public class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
{
    public CreateSupplierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode ter mais de 200 caracteres");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("Documento é obrigatório")
            .Must(BeValidDocument).WithMessage("Documento deve ser um CPF ou CNPJ válido");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email inválido")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }

    private bool BeValidDocument(string document)
    {
        if (string.IsNullOrEmpty(document))
            return false;

        // Remover caracteres não numéricos
        var numbersOnly = Regex.Replace(document, @"[^\d]", "");

        // Validar se é CPF (11 dígitos) ou CNPJ (14 dígitos)
        return numbersOnly.Length == 11 || numbersOnly.Length == 14;
    }
}

