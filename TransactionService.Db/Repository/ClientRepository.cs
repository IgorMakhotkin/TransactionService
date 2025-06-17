using Microsoft.EntityFrameworkCore;
using TransactionService.Db;
using TransactionService.Db.Repository;
using TransactionService.Models.Entity;

namespace TransactionService.Repository;

public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(TransactionDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Client?> CreateClientAsync(CancellationToken cancellationToken = default)
    {
        var clientId = Guid.NewGuid();

        var client = new Client()
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Id = clientId
        };
        return await CreateAsync(new Client(), cancellationToken);
    }

    public async Task<Client?> GetClientAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Balance)
            .FirstOrDefaultAsync(x => x.Id == clientId, cancellationToken);
    }

    public async Task<bool> UpdateClientBalanceAsync(Guid clientId, decimal amount, CancellationToken cancellationToken = default)
    {
        var client = await GetAsync(clientId, cancellationToken);

        if (client is not null)
        {
            client.Balance.Amount = amount;
            await UpdateAsync(client, cancellationToken);
        }

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}