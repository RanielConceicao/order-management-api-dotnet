using FluentValidation;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Validators;

public class CreateProdutoValidator : AbstractValidator<CreateProdutoDto>
{
    public CreateProdutoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome do produto é obrigatório.")
            .MaximumLength(200).WithMessage("Nome do produto não pode exceder 200 caracteres.");

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero.");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição não pode exceder 500 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0).WithMessage("Estoque não pode ser negativo.");
    }
}

public class UpdateProdutoValidator : AbstractValidator<UpdateProdutoDto>
{
    public UpdateProdutoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome do produto é obrigatório.")
            .MaximumLength(200).WithMessage("Nome do produto não pode exceder 200 caracteres.");

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero.");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição não pode exceder 500 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0).WithMessage("Estoque não pode ser negativo.");
    }
}
