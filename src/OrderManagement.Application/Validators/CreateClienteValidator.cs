using FluentValidation;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Validators;

public class CreateClienteValidator : AbstractValidator<CreateClienteDto>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome do cliente é obrigatório.")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email em formato inválido.")
            .MaximumLength(200).WithMessage("Email não pode exceder 200 caracteres.");

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone não pode exceder 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Telefone));
    }
}

public class UpdateClienteValidator : AbstractValidator<UpdateClienteDto>
{
    public UpdateClienteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome do cliente é obrigatório.")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email em formato inválido.")
            .MaximumLength(200).WithMessage("Email não pode exceder 200 caracteres.");

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone não pode exceder 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Telefone));
    }
}
