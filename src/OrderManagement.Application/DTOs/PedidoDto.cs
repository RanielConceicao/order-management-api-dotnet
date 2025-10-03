namespace OrderManagement.Application.DTOs;

public record PedidoDto
{
    public int Id { get; init; }
    public DateTime DataPedido { get; init; }
    public int ClienteId { get; init; }
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record PedidoDetalhesDto
{
    public int Id { get; init; }
    public DateTime DataPedido { get; init; }
    public int ClienteId { get; init; }
    public string ClienteNome { get; init; } = string.Empty;
    public string ClienteEmail { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public List<ItemPedidoDto> Itens { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record ItemPedidoDto
{
    public int Id { get; init; }
    public int ProdutoId { get; init; }
    public string ProdutoNome { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
    public decimal Subtotal { get; init; }
}

public record CreatePedidoDto
{
    public int ClienteId { get; init; }
    public List<CreateItemPedidoDto> Itens { get; init; } = new();
}

public record CreateItemPedidoDto
{
    public int ProdutoId { get; init; }
    public int Quantidade { get; init; }
}

public record UpdatePedidoDto
{
    public int ClienteId { get; init; }
    public List<CreateItemPedidoDto> Itens { get; init; } = new();
}
