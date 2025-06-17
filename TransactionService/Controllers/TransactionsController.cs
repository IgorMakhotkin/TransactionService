using Mapster;
using Microsoft.AspNetCore.Mvc;
using Transaction;
using TransactionService.Extensions;
using TransactionService.Models;
using TransactionService.Models.Models.Response;
using TransactionService.Models.Response;
using TransactionService.Repository;

namespace TransactionService.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController(ITransactionService transactionService, IClientRepository clientRepository, IClientBalanceRepository clientBalanceRepository) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IClientBalanceRepository _clientBalanceRepository = clientBalanceRepository;

    /// <summary>
    /// Создать тестового клиента 
    /// </summary>
    /// <param name="amount">Сумма счета</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("client")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BalanceDtoResponse))]
    public async Task<IActionResult> Client(decimal amount,
    CancellationToken cancellationToken)
    {
        var client = await _clientRepository.CreateClientAsync(cancellationToken);
        var balance = await _clientBalanceRepository.CreateBalanceAsync(client.Id, amount);

        return Ok(balance.Adapt<BalanceDtoResponse>());
    }

    /// <summary>
    /// Зачислить средства клиенту
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("credit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreditResponse))]
    public async Task<IActionResult> Credit(CreditRequest request,
    CancellationToken cancellationToken)
    {
        var result = await _transactionService.Credit(request.Adapt<CreditTransaction>(), cancellationToken);

        return result.IsSuccessStatusCode()
            ? Ok(result.Data)
            : StatusCode((int)result.StatusCode, new ProblemDetails
            {
                Title = result.Message,
                Status = (int)result.StatusCode,
                Detail = result.Message,
                Extensions = { ["errors"] = result.Errors ?? new List<ProblemError>() }
            });
    }

    /// <summary>
    /// Списать средства клиента
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("debit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DebitResponse))]
    public async Task<IActionResult> Debit(DebitRequest request,
    CancellationToken cancellationToken) 
    {
        var result = await _transactionService.Debit(request.Adapt<DebitTransaction>(), cancellationToken);

        return result.IsSuccessStatusCode()
            ? Ok(result.Data)
            : StatusCode((int)result.StatusCode, new ProblemDetails
            {
                Title = result.Message,
                Status = (int)result.StatusCode,
                Detail = result.Message,
                Extensions = { ["errors"] = result.Errors ?? new List<ProblemError>() }
            });
    }

    /// <summary>
    /// Отменить транзакцию
    /// </summary>
    /// <param name="id">id транзакции</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("revert")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreditResponse))]
    public async Task<IActionResult> Revert(Guid id,
    CancellationToken cancellationToken)
    {
        var result = await _transactionService.Revert(id, cancellationToken);

        return result.IsSuccessStatusCode()
            ? Ok(result.Data)
            : StatusCode((int)result.StatusCode, new ProblemDetails
            {
                Title = result.Message,
                Status = (int)result.StatusCode,
                Detail = result.Message,
                Extensions = { ["errors"] = result.Errors ?? new List<ProblemError>() }
            });
    }

    /// <summary>
    /// Получить актуальный баланс клиента
    /// </summary>
    /// <param name="id">id клиента</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("balance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BalanceResponse))]
    public async Task<IActionResult> Balance(Guid id,
    CancellationToken cancellationToken)
    {
        var result = await _transactionService.GetBalance(id, cancellationToken);

        return result.IsSuccessStatusCode()
          ? Ok(result.Data)
          : StatusCode((int)result.StatusCode, new ProblemDetails
          {
              Title = result.Message,
              Status = (int)result.StatusCode,
              Detail = result.Message,
              Extensions = { ["errors"] = result.Errors ?? new List<ProblemError>() }
          });
    }
}