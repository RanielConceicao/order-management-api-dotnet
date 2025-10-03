using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pedido>> GetByClienteIdAsync(int clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pedido>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pedido>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
}
