using Mapster;
using System.Net;
using TransactionService.Models;
using TransactionService.Models.Constants;
using TransactionService.Models.Entity;
using TransactionService.Models.Models.Entity;
using TransactionService.Models.Models.Response;
using TransactionService.Models.Response;
using TransactionService.Repository;

namespace Transaction;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IClientBalanceRepository _clientBalanceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository transactionRepository,
        IClientBalanceRepository clientBalanceRepository,
        IClientRepository clientRepository,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _clientBalanceRepository = clientBalanceRepository;
        _clientRepository = clientRepository;
        _logger = logger;
    }


    public async Task<HttpDataResult<CreditResponse>> Credit(CreditTransaction request, CancellationToken cancellationToken)
    {

        return await ProcessTransactionAsync<CreditTransaction, CreditResponse>(
        request,
        async (req, client, existingTrans) =>
        {
            var transactionRecord = new TransactionRecord
            {
                Id = request.Id,
                ClientId = client.Id,
                Client = client,
                Amount = request.Amount,
                Type = TransactionType.Credit,
                Status = TransactionStatus.Applied,
                CreatedAt = DateTime.UtcNow
            };

            client.Balance.Amount += request.Amount;

            try
            {
                await _transactionRepository.CreateTransactionAsync(transactionRecord, cancellationToken);
                await _clientRepository.UpdateClientBalanceAsync(client.Id, client.Balance.Amount, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка сохранения транзакции", ex.Message);
                return new HttpDataResult<CreditResponse>(
                    HttpStatusCode.InternalServerError,
                    "Ошибка сохранения транзакции");
            }

            _logger.LogInformation(
                $"Счет пополнен на {request.Amount} клиенту с id:  {client.Id}. новый баланс {client.Balance.Amount}",
                request.Amount,
                client.Id,
                client.Balance);

            return new HttpDataResult<CreditResponse>(
                new CreditResponse
                {
                    InsertDateTime = DateTime.UtcNow,
                    ClientBalance = client.Balance.Amount
                },
                HttpStatusCode.OK);
        },
        (client, existingTrans) => new CreditResponse
        {
            InsertDateTime = existingTrans.CreatedAt,
            ClientBalance = client.Balance.Amount
        },
        cancellationToken);
    }

    public async Task<HttpDataResult<DebitResponse>> Debit(DebitTransaction request, CancellationToken cancellationToken = default)
    {
        return await ProcessTransactionAsync<DebitTransaction, DebitResponse>(
        request,
        async (req, client, existingTrans) =>
        {
            var transactionRecord = new TransactionRecord
            {
                Id = request.Id,
                ClientId = client.Id,
                Client = client,
                Amount = request.Amount,
                Type = TransactionType.Debit,
                Status = TransactionStatus.Applied,
                CreatedAt = DateTime.UtcNow
            };

            if(client.Balance.Amount < request.Amount)
            {
                _logger.LogError($"Не достаточная сумма у клиента");
                return new HttpDataResult<DebitResponse>(
                    HttpStatusCode.BadRequest,
                    string.Format(ErrorMessagesConstants.NoMoney, client.Balance.Amount));
            }
            client.Balance.Amount -= request.Amount;

            try
            {
                await _transactionRepository.CreateTransactionAsync(transactionRecord, cancellationToken);
                await _clientRepository.UpdateClientBalanceAsync(client.Id, client.Balance.Amount, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка сохранения транзакции", ex.Message);
                return new HttpDataResult<DebitResponse>(
                    HttpStatusCode.InternalServerError,
                    "Ошибка сохранения транзакции");
            }

            _logger.LogInformation(
                $"Списали {request.Amount} со счеита клиента с id:  {client.Id}. новый баланс {client.Balance.Amount}");

            return new HttpDataResult<DebitResponse>(
                new DebitResponse
                {
                    InsertDateTime = DateTime.UtcNow,
                    ClientBalance = client.Balance.Amount
                },
                HttpStatusCode.OK);
        },
        (client, existingTrans) => new DebitResponse
        {
            InsertDateTime = existingTrans.CreatedAt,
            ClientBalance = client.Balance.Amount
        },
        cancellationToken);
    }

    public async Task<HttpDataResult<BalanceResponse>> GetBalance(Guid clientId, CancellationToken cancellationToken)
    {
        var balance = await _clientBalanceRepository.GetAsync(clientId, cancellationToken);

        if (balance is null)
        {
            return new HttpDataResult<BalanceResponse>(
                HttpStatusCode.NotFound,
                string.Format(ErrorMessagesConstants.BalanceNotFound, clientId));
        }

        return new HttpDataResult<BalanceResponse>(balance.Adapt<BalanceResponse>(), HttpStatusCode.OK);
    }

    public async Task<HttpDataResult<RevertResponse>> Revert(Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);
        if (transaction is null)
        {
            _logger.LogError($"Транзакция не найдена {id}");
            return new HttpDataResult<RevertResponse>(
                HttpStatusCode.NotFound,
                string.Format(ErrorMessagesConstants.TransactionNotFound, id));
        }

        var client = await _clientRepository.GetClientAsync(transaction.ClientId, cancellationToken);
        if (client == null)
        {
            _logger.LogError($"Не найден клиент {transaction.ClientId}");
            return new HttpDataResult<RevertResponse>(
                HttpStatusCode.NotFound,
                string.Format(ErrorMessagesConstants.ClientNotFound, transaction.ClientId));
        }

        _logger.LogInformation($"Отменяем транзакцию с id: {transaction.Id}");

        switch (transaction.Type)
        {
            case TransactionType.Credit:

                if (client.Balance.Amount < transaction.Amount)
                {
                    _logger.LogError($"Не достаточная сумма у клиента");
                    return new HttpDataResult<RevertResponse>(
                        HttpStatusCode.BadRequest,
                        string.Format(ErrorMessagesConstants.NoMoney, client.Balance.Amount));
                }

                client.Balance.Amount -= transaction.Amount;

                try
                {
                    await _transactionRepository.DeleteTransactionAsync(transaction, cancellationToken);
                    await _clientRepository.UpdateClientBalanceAsync(client.Id, client.Balance.Amount, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Ошибка отмены транзакции", ex.Message);
                    return new HttpDataResult<RevertResponse>(
                        HttpStatusCode.InternalServerError,
                        "Ошибка отмены транзакции");
                }

                break;

            case TransactionType.Debit:

                client.Balance.Amount += transaction.Amount;

                try
                {
                    await _transactionRepository.DeleteTransactionAsync(transaction, cancellationToken);
                    await _clientRepository.UpdateClientBalanceAsync(client.Id, client.Balance.Amount, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Ошибка отмены транзакции", ex.Message);
                    return new HttpDataResult<RevertResponse>(
                        HttpStatusCode.InternalServerError,
                        "Ошибка отмены транзакции");
                }

                break;

            default:
                break;
        }

        _logger.LogInformation(
        $"Отменили транзакцию  {transaction.Id} для клиента с id: {client.Id}. новый баланс {client.Balance.Amount}");

        return new HttpDataResult<RevertResponse>(
        new RevertResponse
        {
            RevertDateTime = DateTime.UtcNow,
            ClientBalance = client.Balance.Amount
        },
        HttpStatusCode.OK);
    }

    private async Task<HttpDataResult<TResponse>> ProcessTransactionAsync<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, Client, TransactionRecord, Task<HttpDataResult<TResponse>>> processNewTransaction,
        Func<Client, TransactionRecord, TResponse> createExistingTransactionResponse,
        CancellationToken cancellationToken)
        where TRequest : ITransaction
    {
        try
        {
            var existingTransaction = await _transactionRepository
                .GetByIdAsync(request.Id, cancellationToken);

            var client = await _clientRepository.GetClientAsync(request.ClientId, cancellationToken);
            if (client == null)
            {
                _logger.LogError("Не найден клиент {ClientId}", request.ClientId);
                return new HttpDataResult<TResponse>(
                    HttpStatusCode.NotFound,
                    string.Format(ErrorMessagesConstants.ClientNotFound, request.ClientId));
            }

            if (existingTransaction != null)
            {

                _logger.LogWarning(
                    $"Транзакция уже выполнена TransactionId: {request.Id}, " +
                    $"ClientId: {request.ClientId}, " +
                    $"Amount: {existingTransaction.Amount}, " +
                    $"CreatedAt: {existingTransaction.CreatedAt}");

                var response = createExistingTransactionResponse(client, existingTransaction);

                return new HttpDataResult<TResponse>(
                    response,
                    HttpStatusCode.PartialContent,
                    string.Format(ErrorMessagesConstants.TransactionAlreadyProcessed, request.Id));
            }

            return await processNewTransaction(request, client, existingTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка транзакции пополнения счета");
            return new HttpDataResult<TResponse>(
                HttpStatusCode.InternalServerError,
                "Ошибка транзакции пополнения счета");
        }
    }
}