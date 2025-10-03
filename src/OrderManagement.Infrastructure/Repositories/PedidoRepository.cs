using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Pedidos.
/// Inclui métodos para carregar pedidos com seus relacionamentos (eager loading).
/// </summary>
public class PedidoRepository : Repository<Pedido>, IPedidoRepository
{
    public PedidoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Pedido?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Pedido>> GetByClienteIdAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ClienteId == clienteId)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Pedido>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.DataPedido >= startDate && p.DataPedido <= endDate)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Pedido>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync(cancellationToken);
    }
}
