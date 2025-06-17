namespace TransactionService.Models;

public class RevertTransaction : ITransaction
{
    public Guid RevertedTransactionId { get; set; }
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Amount { get; set; }
}