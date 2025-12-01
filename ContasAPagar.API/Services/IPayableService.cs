using ContasAPagar.API.DTOs.Common;
using ContasAPagar.API.DTOs.Payable;

namespace ContasAPagar.API.Services;

public interface IPayableService
{
    Task<PayableResponse> CreateAsync(CreatePayableRequest request);
    Task<PayableResponse?> GetByIdAsync(string id);
    Task<PagedResponse<PayableResponse>> GetFilteredAsync(PayableFilterRequest filter);
    Task<PayableResponse> MarkAsPaidAsync(string id);
    Task<PayableResponse> CancelAsync(string id);
}

