using TransactionService.Models.Models.Entity;

namespace TransactionService.Models.Entity;

public class TransactionRecord
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Client Client { get; set; }

    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? RevertedAt { get; set; }
}