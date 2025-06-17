using Microsoft.EntityFrameworkCore;
using TransactionService.Db;
using TransactionService.Db.Repository;
using TransactionService.Models.Entity;

namespace TransactionService.Repository;

public class TransactionRepository : BaseRepository<TransactionRecord>, ITransactionRepository
{
    public TransactionRepository(TransactionDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<TransactionRecord?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default) => await _dbSet
            .Include(t => t.Client)
            .FirstOrDefaultAsync(x => x.Id == transactionId, cancellationToken);

    public async Task CreateTransactionAsync(TransactionRecord transaction, CancellationToken cancellationToken = default)
    {
        await CreateAsync(transaction, cancellationToken);
    }

    public async Task DeleteTransactionAsync(TransactionRecord transaction, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(transaction, cancellationToken);
    }
}