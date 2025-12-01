namespace ContasAPagar.API.DTOs.Payable;

public class CreatePayableRequest
{
    public string SupplierId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
}

