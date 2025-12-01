using ContasAPagar.API.Models;

namespace ContasAPagar.API.DTOs.Payable;

public class PayableResponse
{
    public string Id { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

