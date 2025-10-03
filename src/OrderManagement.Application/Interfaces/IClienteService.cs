using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Interfaces;

public interface IClienteService
{
    Task<ClienteDto?> GetByIdAsync(int id);
    Task<IEnumerable<ClienteDto>> GetAllAsync();
    Task<ClienteDto> CreateAsync(CreateClienteDto dto);
    Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto dto);
    Task DeleteAsync(int id);
} 
