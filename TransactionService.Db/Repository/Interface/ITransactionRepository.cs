using TransactionService.Models.Entity;

namespace TransactionService.Repository;

public interface ITransactionRepository
{
    Task<TransactionRecord?> GetByIdAsync(Guid transactionId,
        CancellationToken cancellationToken = default);

    Task CreateTransactionAsync(TransactionRecord transaction, 
        CancellationToken cancellationToken = default);

    Task DeleteTransactionAsync(TransactionRecord transaction,
        CancellationToken cancellationToken = default);
}