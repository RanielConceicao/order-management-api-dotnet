using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Entities;

public class Produto : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }
    public string? Descricao { get; private set; }
    public int Estoque { get; private set; }

    private Produto() { }

    public Produto(string nome, decimal preco, string? descricao, int estoque)
    {
        ValidateProductData(nome, preco, estoque);
        
        Nome = nome;
        Preco = preco;
        Descricao = descricao;
        Estoque = estoque;
    }

    public void Update(string nome, decimal preco, string? descricao, int estoque)
    {
        ValidateProductData(nome, preco, estoque);
        
        Nome = nome;
        Preco = preco;
        Descricao = descricao;
        Estoque = estoque;
        UpdateTimestamp();
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantity));

        if (Estoque < quantity)
            throw new InvalidOperationException($"Estoque insuficiente. Disponível: {Estoque}, Solicitado: {quantity}");

        Estoque -= quantity;
        UpdateTimestamp();
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantity));

        Estoque += quantity;
        UpdateTimestamp();
    }

    private static void ValidateProductData(string nome, decimal preco, int estoque)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório.", nameof(nome));

        if (preco <= 0)
            throw new ArgumentException("Preço deve ser maior que zero.", nameof(preco));

        if (estoque < 0)
            throw new ArgumentException("Estoque não pode ser negativo.", nameof(estoque));
    }
}
