using TransactionService.Models.Entity;

namespace TransactionService.Repository;

public interface IClientBalanceRepository
{
    Task<Balance> GetAsync(Guid clientId,
        CancellationToken cancellationToken = default);

    Task<Balance> CreateBalanceAsync(Guid clientId, decimal amount,
        CancellationToken cancellationToken = default);
}