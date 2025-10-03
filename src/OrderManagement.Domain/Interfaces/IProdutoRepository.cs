using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IEnumerable<Produto>> GetByNameAsync(string nome, CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> GetInStockAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
