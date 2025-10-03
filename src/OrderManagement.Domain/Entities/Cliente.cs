using OrderManagement.Domain.Common;
using OrderManagement.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Domain.Entities;

public class Cliente : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }

    private Cliente() { }

    public Cliente(string nome, string email, string? telefone = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do cliente é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório.", nameof(email));

    // Validate and normalize email using Value Object
    var emailVo = OrderManagement.Domain.ValueObjects.Email.Create(email);

        Nome = nome;
        Email = emailVo.Value;
        Telefone = telefone;
    }

    public void Update(string nome, string email, string? telefone = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do cliente é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório.", nameof(email));

    // Validate and normalize email using Value Object
    var emailVo = OrderManagement.Domain.ValueObjects.Email.Create(email);

        Nome = nome;
        Email = emailVo.Value;
        Telefone = telefone;
        UpdateTimestamp();
    }
} 
