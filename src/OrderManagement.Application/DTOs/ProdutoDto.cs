namespace OrderManagement.Application.DTOs;

public record ProdutoDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public decimal Preco { get; init; }
    public string? Descricao { get; init; }
    public int Estoque { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateProdutoDto
{
    public string Nome { get; init; } = string.Empty;
    public decimal Preco { get; init; }
    public string? Descricao { get; init; }
    public int Estoque { get; init; }
}

public record UpdateProdutoDto
{
    public string Nome { get; init; } = string.Empty;
    public decimal Preco { get; init; }
    public string? Descricao { get; init; }
    public int Estoque { get; init; }
}
