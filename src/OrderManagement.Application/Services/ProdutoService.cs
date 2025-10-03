using AutoMapper;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProdutoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProdutoDto?> GetByIdAsync(int id)
    {
        var produtoEntity = await _unitOfWork.Produtos.GetByIdAsync(id);
        return produtoEntity == null ? null : _mapper.Map<ProdutoDto>(produtoEntity);
    }

    public async Task<IEnumerable<ProdutoDto>> GetAllAsync()
    {
        var produtosEntity = await _unitOfWork.Produtos.GetAllAsync();
        return _mapper.Map<IEnumerable<ProdutoDto>>(produtosEntity);
    }

    public async Task<ProdutoDto> CreateAsync(CreateProdutoDto dto)
    {
        var produtoEntity = new Produto(dto.Nome, dto.Preco, dto.Descricao, dto.Estoque);
        var createdEntity = await _unitOfWork.Produtos.AddAsync(produtoEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProdutoDto>(createdEntity);
    }

    public async Task<ProdutoDto> UpdateAsync(int id, UpdateProdutoDto dto)
    {
        var produtoEntity = await _unitOfWork.Produtos.GetByIdAsync(id);
        if (produtoEntity == null)
        {
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
        }

        produtoEntity.Update(dto.Nome, dto.Preco, dto.Descricao, dto.Estoque);
        await _unitOfWork.Produtos.UpdateAsync(produtoEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProdutoDto>(produtoEntity);
    }

    public async Task DeleteAsync(int id)
    {
        var produtoEntity = await _unitOfWork.Produtos.GetByIdAsync(id);
        if (produtoEntity == null)
        {
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
        }

        await _unitOfWork.Produtos.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
} 
