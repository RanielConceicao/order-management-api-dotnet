namespace OrderManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProdutoRepository Produtos { get; }
    IClienteRepository Clientes { get; }
    IPedidoRepository Pedidos { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
