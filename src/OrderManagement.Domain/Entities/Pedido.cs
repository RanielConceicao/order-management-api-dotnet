using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Entities;

public class Pedido : BaseEntity
{
    public DateTime DataPedido { get; private set; }
    public int ClienteId { get; private set; }
    public decimal Total { get; private set; }

    public Cliente Cliente { get; private set; } = null!;
    
    private readonly List<ItemPedido> _itens = new();
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    private Pedido() { }

    public Pedido(int clienteId, List<ItemPedido> itens)
    {
        if (itens == null || itens.Count == 0)
            throw new ArgumentException("Pedido deve conter pelo menos um item.", nameof(itens));

        ClienteId = clienteId;
        DataPedido = DateTime.UtcNow;
        _itens = itens;
        CalculateTotal();
    }

    public void AddItem(ItemPedido item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _itens.Add(item);
        CalculateTotal();
        UpdateTimestamp();
    }

    public void RemoveItem(int itemId)
    {
        if (_itens.Count <= 1)
            throw new InvalidOperationException("Pedido deve conter pelo menos um item. Não é possível remover o último item.");

        var itemToRemove = _itens.FirstOrDefault(i => i.Id == itemId);
        if (itemToRemove == null)
            throw new ArgumentException($"Item com ID {itemId} não encontrado no pedido.", nameof(itemId));

        _itens.Remove(itemToRemove);
        CalculateTotal();
        UpdateTimestamp();
    }

    public void UpdateItems(List<ItemPedido> novosItens)
    {
        if (novosItens == null || novosItens.Count == 0)
            throw new ArgumentException("Pedido deve conter pelo menos um item.", nameof(novosItens));

        _itens.Clear();
        _itens.AddRange(novosItens);
        CalculateTotal();
        UpdateTimestamp();
    }

    private void CalculateTotal()
    {
        Total = _itens.Sum(item => item.Subtotal);
    }

    public bool IsValid()
    {
        return _itens.Count > 0 && Total > 0;
    }
}
