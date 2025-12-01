namespace ContasAPagar.API.DTOs.Payable;

public class PayableFilterRequest
{
    public string? SupplierId { get; set; }
    public DateTime? StartDueDate { get; set; }
    public DateTime? EndDueDate { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

