using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Entities;

public class ItemPedido : BaseEntity
{
    public int PedidoId { get; private set; }
    public int ProdutoId { get; private set; }
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }

    public Pedido Pedido { get; private set; } = null!;
    public Produto Produto { get; private set; } = null!;

    public decimal Subtotal => Quantidade * PrecoUnitario;

    private ItemPedido() { }

    public ItemPedido(int produtoId, int quantidade, decimal precoUnitario)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero.", nameof(precoUnitario));

        ProdutoId = produtoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    public void UpdateQuantity(int novaQuantidade)
    {
        if (novaQuantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(novaQuantidade));

        Quantidade = novaQuantidade;
        UpdateTimestamp();
    }
}
