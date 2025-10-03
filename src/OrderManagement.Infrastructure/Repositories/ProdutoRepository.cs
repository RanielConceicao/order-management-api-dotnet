using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> GetByNameAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Nome.Contains(nome))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> GetInStockAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Estoque > 0)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Preco >= minPrice && p.Preco <= maxPrice)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (!idList.Any()) return Enumerable.Empty<Produto>();

        return await _dbSet
            .Where(p => idList.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }
}
