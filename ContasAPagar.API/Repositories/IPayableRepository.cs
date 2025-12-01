using ContasAPagar.API.DTOs.Payable;
using ContasAPagar.API.Models;

namespace ContasAPagar.API.Repositories;

public interface IPayableRepository
{
    Task<Payable> CreateAsync(Payable payable);
    Task<Payable?> GetByIdAsync(string id);
    Task<(List<Payable> Items, int TotalCount)> GetFilteredAsync(PayableFilterRequest filter);
    Task<bool> UpdateAsync(Payable payable);
}

