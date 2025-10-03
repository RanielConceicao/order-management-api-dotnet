using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Interfaces;

public interface IPedidoService
{
    Task<PedidoDetalhesDto?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PedidoDetalhesDto>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<PedidoDto> CreateAsync(CreatePedidoDto dto, CancellationToken cancellationToken = default);
    Task<PedidoDto> UpdateAsync(int id, UpdatePedidoDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PedidoDto>> GetByClienteIdAsync(int clienteId, CancellationToken cancellationToken = default);
}
