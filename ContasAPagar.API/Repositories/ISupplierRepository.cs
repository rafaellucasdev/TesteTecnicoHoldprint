using ContasAPagar.API.Models;

namespace ContasAPagar.API.Repositories;

public interface ISupplierRepository
{
    Task<Supplier> CreateAsync(Supplier supplier);
    Task<Supplier?> GetByIdAsync(string id);
    Task<Supplier?> GetByDocumentAsync(string document);
    Task<List<Supplier>> GetAllAsync();
    Task<bool> ExistsAsync(string id);
}

