using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Persistence;
 
namespace OrderManagement.Infrastructure.Repositories;

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cliente?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Email == email.ToLowerInvariant())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> GetByNameAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Nome.Contains(nome))
            .ToListAsync(cancellationToken);
    }
}
