using FluentAssertions;
using OrderManagement.Domain.Entities;
using Xunit;

namespace OrderManagement.Tests.Domain;

public class ClienteEmailTests
{
    [Theory]
    [InlineData("teste@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("user+tag@example.com")]
    public void Cliente_DeveCriar_QuandoEmailValido(string emailValido)
    {
        var cliente = new Cliente("João Silva", emailValido, "11999999999");

        cliente.Should().NotBeNull();
        cliente.Email.Should().Be(emailValido.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Cliente_DeveLancarExcecao_QuandoEmailVazioOuNulo(string? emailInvalido)
    {
        var act = () => new Cliente("João Silva", emailInvalido!, "11999999999");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Email é obrigatório.*");
    }

    [Theory]
    [InlineData("emailsemarroba")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user @example.com")]
    public void Cliente_DeveLancarExcecao_QuandoEmailFormatoInvalido(string emailInvalido)
    {
        var act = () => new Cliente("João Silva", emailInvalido, "11999999999");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Formato de email inválido.*");
    }

    [Fact]
    public void Cliente_DeveNormalizarEmail_QuandoCriado()
    {
        var cliente = new Cliente("João Silva", "TESTE@EXAMPLE.COM", "11999999999");

        cliente.Email.Should().Be("teste@example.com");
    }

    [Fact]
    public void Cliente_DeveAtualizarEmail_QuandoValido()
    {
        var cliente = new Cliente("João Silva", "teste@example.com", "11999999999");

        cliente.Update("João Silva", "NOVO@EXAMPLE.COM", "11999999999");

        cliente.Email.Should().Be("novo@example.com");
    }
}
