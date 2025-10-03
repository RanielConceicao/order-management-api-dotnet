using FluentAssertions;
using OrderManagement.Domain.Entities;
using Xunit;

namespace OrderManagement.Tests.Domain;

public class ProdutoTests
{
    [Fact]
    public void Produto_DeveLancarExcecao_QuandoNomeVazio()
    {
        var act = () => new Produto(nome: "", preco: 10.00m, descricao: "Teste", estoque: 5);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Nome do produto é obrigatório.*");
    }

    [Fact]
    public void Produto_DeveLancarExcecao_QuandoPrecoZeroOuNegativo()
    {
        var act = () => new Produto(nome: "Produto Teste", preco: 0, descricao: "Teste", estoque: 5);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Preço deve ser maior que zero.*");
    }

    [Fact]
    public void Produto_DeveLancarExcecao_QuandoEstoqueNegativo()
    {
        var act = () => new Produto(nome: "Produto Teste", preco: 10.00m, descricao: "Teste", estoque: -1);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Estoque não pode ser negativo.*");
    }

    [Fact]
    public void Produto_DeveReduzirEstoque_QuandoQuantidadeValida()
    {
        var produto = new Produto(nome: "Produto Teste", preco: 10.00m, descricao: "Teste", estoque: 10);

        produto.ReduceStock(3);

        produto.Estoque.Should().Be(7);
    }

    [Fact]
    public void Produto_DeveLancarExcecao_QuandoEstoqueInsuficiente()
    {
        var produto = new Produto(nome: "Produto Teste", preco: 10.00m, descricao: "Teste", estoque: 5);

        var act = () => produto.ReduceStock(10);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Estoque insuficiente.*");
    }

    [Fact]
    public void Produto_DeveAumentarEstoque_QuandoQuantidadeValida()
    {
        var produto = new Produto(nome: "Produto Teste", preco: 10.00m, descricao: "Teste", estoque: 5);

        produto.IncreaseStock(10);

        produto.Estoque.Should().Be(15);
    }

    [Fact]
    public void Produto_DeveAtualizarDados_Corretamente()
    {
        var produto = new Produto(nome: "Produto Original", preco: 10.00m, descricao: "Desc Original", estoque: 5);

        produto.Update(nome: "Produto Atualizado", preco: 15.00m, descricao: "Desc Atualizada", estoque: 10);

        produto.Nome.Should().Be("Produto Atualizado");
        produto.Preco.Should().Be(15.00m);
        produto.Descricao.Should().Be("Desc Atualizada");
        produto.Estoque.Should().Be(10);
    }
}
