using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace TransactionService.Models;

public class CreditRequest
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }

    public DateTime DateTime { get; set; }

    public decimal Amount { get; set; }

}

public class CreditRequestValidator: AbstractValidator<CreditRequest>
{
    public CreditRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id транзакции обязателен");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ClientId обязателен");

        RuleFor(x => x.DateTime)
            .NotEmpty().WithMessage("Дата обязательна")
            .NotNull().WithMessage("Дата обязательна")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Дата транзакции не может быть в будущем");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Сумма транзакции должна быть положительной");
    }
}