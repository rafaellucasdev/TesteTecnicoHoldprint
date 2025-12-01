using ContasAPagar.API.DTOs.Supplier;

namespace ContasAPagar.API.Services;

public interface ISupplierService
{
    Task<SupplierResponse> CreateAsync(CreateSupplierRequest request);
    Task<SupplierResponse?> GetByIdAsync(string id);
    Task<List<SupplierResponse>> GetAllAsync();
}

