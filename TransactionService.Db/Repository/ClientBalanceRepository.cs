using TransactionService.Db;
using TransactionService.Db.Repository;
using TransactionService.Models.Entity;

namespace TransactionService.Repository;

public class ClientBalanceRepository : BaseRepository<Balance>, IClientBalanceRepository
{
    public ClientBalanceRepository(TransactionDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Balance> CreateBalanceAsync(Guid clientId, decimal amount, CancellationToken cancellationToken = default)
    {
        var balance = new Balance()
        {
            ClientId = clientId,
            Amount = amount
        };

       return await CreateAsync(balance, cancellationToken);
    }
}