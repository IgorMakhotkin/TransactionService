namespace TransactionService.Models.Response;

public class RevertResponse
{
    public DateTime RevertDateTime { get; set; }
    public decimal ClientBalance { get; set; }
}