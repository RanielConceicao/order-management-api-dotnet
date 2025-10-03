using FluentAssertions;
using OrderManagement.Domain.Entities;
using Xunit;

namespace OrderManagement.Tests.Domain;

public class PedidoTests
{
    [Fact]
    public void Pedido_DeveLancarExcecao_QuandoCriadoSemItens()
    {
        var act = () => new Pedido(clienteId: 1, new List<ItemPedido>());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Pedido deve conter pelo menos um item.*");
    }

    [Fact]
    public void Pedido_DeveCalcularTotalCorretamente_QuandoCriadoComItens()
    {
        var itens = new List<ItemPedido>
        {
            new ItemPedido(produtoId: 1, quantidade: 2, precoUnitario: 10.50m),
            new ItemPedido(produtoId: 2, quantidade: 3, precoUnitario: 5.00m)
        };

        var pedido = new Pedido(clienteId: 1, itens);

        pedido.Total.Should().Be(36.00m);
        pedido.Itens.Should().HaveCount(2);
    }

    [Fact]
    public void Pedido_DeveAdicionarItem_ERecalcularTotal()
    {
        var pedido = new Pedido(clienteId: 1, new List<ItemPedido>
        {
            new ItemPedido(produtoId: 1, quantidade: 1, precoUnitario: 10.00m)
        });

        pedido.AddItem(new ItemPedido(produtoId: 2, quantidade: 2, precoUnitario: 5.00m));

        pedido.Itens.Should().HaveCount(2);
        pedido.Total.Should().Be(20.00m);
    }

    [Fact]
    public void Pedido_DeveLancarExcecao_AoRemoverUltimoItem()
    {
        var pedido = new Pedido(clienteId: 1, new List<ItemPedido>
        {
            new ItemPedido(produtoId: 1, quantidade: 1, precoUnitario: 10.00m)
        });
        var itemId = pedido.Itens.First().Id;

        var act = () => pedido.RemoveItem(itemId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Pedido deve conter pelo menos um item.*");
    }

    [Fact]
    public void Pedido_DeveAtualizarItens_ERecalcularTotal()
    {
        var pedido = new Pedido(clienteId: 1, new List<ItemPedido>
        {
            new ItemPedido(produtoId: 1, quantidade: 1, precoUnitario: 10.00m)
        });

        pedido.UpdateItems(new List<ItemPedido>
        {
            new ItemPedido(produtoId: 2, quantidade: 3, precoUnitario: 7.50m),
            new ItemPedido(produtoId: 3, quantidade: 1, precoUnitario: 15.00m)
        });

        pedido.Itens.Should().HaveCount(2);
        pedido.Total.Should().Be(37.50m);
    }

    [Fact]
    public void Pedido_IsValid_DeveRetornarTrue_QuandoPedidoValido()
    {
        var pedido = new Pedido(clienteId: 1, new List<ItemPedido>
        {
            new ItemPedido(produtoId: 1, quantidade: 1, precoUnitario: 10.00m)
        });

        pedido.IsValid().Should().BeTrue();
    }
}
