using FluentValidation;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Validators;

public class CreatePedidoValidator : AbstractValidator<CreatePedidoDto>
{
    public CreatePedidoValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("ClienteId deve ser maior que zero.");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("Pedido deve conter pelo menos um item.");

        RuleForEach(x => x.Itens).SetValidator(new CreateItemPedidoValidator());
    }
}

public class CreateItemPedidoValidator : AbstractValidator<CreateItemPedidoDto>
{
    public CreateItemPedidoValidator()
    {
        RuleFor(x => x.ProdutoId)
            .GreaterThan(0).WithMessage("ProdutoId deve ser maior que zero.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");
    }
}

public class UpdatePedidoValidator : AbstractValidator<UpdatePedidoDto>
{
    public UpdatePedidoValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("ClienteId deve ser maior que zero.");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("Pedido deve conter pelo menos um item.");

        RuleForEach(x => x.Itens).SetValidator(new CreateItemPedidoValidator());
    }
}
