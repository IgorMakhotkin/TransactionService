namespace TransactionService.Models.Response;

public class DebitResponse
{
    public DateTime InsertDateTime { get; set; }
    public decimal ClientBalance { get; set; }
}