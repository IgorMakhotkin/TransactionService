using TransactionService.Models;
using TransactionService.Models.Models.Response;
using TransactionService.Models.Response;

namespace Transaction;

public interface ITransactionService
{
    Task<HttpDataResult<BalanceResponse>> GetBalance(Guid clientId, CancellationToken cancellationToken = default);

    Task<HttpDataResult<CreditResponse>> Credit(CreditTransaction request, CancellationToken cancellationToken = default);

    Task<HttpDataResult<DebitResponse>> Debit(DebitTransaction request, CancellationToken cancellationToken = default);

    Task<HttpDataResult<RevertResponse>> Revert(Guid transactionId, CancellationToken cancellationToken = default);
}