using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IProdutoRepository? _produtos;
    private IClienteRepository? _clientes;
    private IPedidoRepository? _pedidos;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProdutoRepository Produtos
    {
        get
        {
            _produtos ??= new ProdutoRepository(_context);
            return _produtos;
        }
    }

    public IClienteRepository Clientes
    {
        get
        {
            _clientes ??= new ClienteRepository(_context);
            return _clientes;
        }
    }

    public IPedidoRepository Pedidos
    {
        get
        {
            _pedidos ??= new PedidoRepository(_context);
            return _pedidos;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.RollbackTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
