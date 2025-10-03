using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cliente>> GetByNameAsync(string nome, CancellationToken cancellationToken = default);
}
