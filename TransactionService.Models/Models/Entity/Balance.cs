using System.ComponentModel.DataAnnotations;

namespace TransactionService.Models.Entity;

public class Balance
{
    [Key]
    public Guid ClientId { get; set; }
    public decimal Amount { get; set; }

    public Client Client { get; set; }
}