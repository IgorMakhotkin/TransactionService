using TransactionService.Models.Entity;

namespace TransactionService.Repository;

public interface IClientRepository
{
    Task<Client?> GetClientAsync(Guid clientId,
        CancellationToken cancellationToken = default);

    Task<Client?> CreateClientAsync(CancellationToken cancellationToken = default);

    Task<bool> UpdateClientBalanceAsync(Guid clientId, decimal amount, 
        CancellationToken cancellationToken = default);
}