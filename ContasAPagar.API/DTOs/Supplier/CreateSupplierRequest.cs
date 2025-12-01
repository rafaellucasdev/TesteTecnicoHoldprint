namespace ContasAPagar.API.DTOs.Supplier;

public class CreateSupplierRequest
{
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string? Email { get; set; }
}

