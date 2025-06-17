namespace TransactionService.Models.Entity;

public class Client
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Balance Balance { get; set; }
    public List<TransactionRecord> Transactions { get; set; } = new();
}