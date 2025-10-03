using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Interfaces;

public interface IProdutoService
{
    Task<ProdutoDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProdutoDto>> GetAllAsync();
    Task<ProdutoDto> CreateAsync(CreateProdutoDto dto);
    Task<ProdutoDto> UpdateAsync(int id, UpdateProdutoDto dto);
    Task DeleteAsync(int id);
} 
