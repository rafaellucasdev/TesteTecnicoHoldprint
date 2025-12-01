using ContasAPagar.API.DTOs.Common;
using ContasAPagar.API.DTOs.Payable;
using ContasAPagar.API.Exceptions;
using ContasAPagar.API.Models;
using ContasAPagar.API.Repositories;

namespace ContasAPagar.API.Services;

public class PayableService : IPayableService
{
    private readonly IPayableRepository _payableRepository;
    private readonly ISupplierRepository _supplierRepository;

    public PayableService(
        IPayableRepository payableRepository,
        ISupplierRepository supplierRepository)
    {
        _payableRepository = payableRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<PayableResponse> CreateAsync(CreatePayableRequest request)
    {
        // Verificar se o fornecedor existe
        var supplierExists = await _supplierRepository.ExistsAsync(request.SupplierId);
        if (!supplierExists)
        {
            throw new BusinessException($"Fornecedor com ID {request.SupplierId} não foi encontrado");
        }

        var payable = new Payable
        {
            SupplierId = request.SupplierId,
            Description = request.Description,
            DueDate = request.DueDate,
            Amount = request.Amount,
            Status = PayableStatus.Pending, // Status inicial sempre Pending
            CreatedAt = DateTime.UtcNow
        };

        var created = await _payableRepository.CreateAsync(payable);
        return MapToResponse(created);
    }

    public async Task<PayableResponse?> GetByIdAsync(string id)
    {
        var payable = await _payableRepository.GetByIdAsync(id);
        return payable != null ? MapToResponse(payable) : null;
    }

    public async Task<PagedResponse<PayableResponse>> GetFilteredAsync(PayableFilterRequest filter)
    {
        var (items, totalCount) = await _payableRepository.GetFilteredAsync(filter);

        return new PagedResponse<PayableResponse>
        {
            Data = items.Select(MapToResponse).ToList(),
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PayableResponse> MarkAsPaidAsync(string id)
    {
        var payable = await _payableRepository.GetByIdAsync(id);
        if (payable == null)
        {
            throw new NotFoundException($"Conta a pagar com ID {id} não foi encontrada");
        }

        // Validar se a conta pode ser marcada como paga
        if (payable.Status == PayableStatus.Paid)
        {
            throw new BusinessException("Esta conta já foi paga");
        }

        if (payable.Status == PayableStatus.Canceled)
        {
            throw new BusinessException("Não é possível pagar uma conta cancelada");
        }

        payable.Status = PayableStatus.Paid;
        payable.PaymentDate = DateTime.UtcNow;

        await _payableRepository.UpdateAsync(payable);
        return MapToResponse(payable);
    }

    public async Task<PayableResponse> CancelAsync(string id)
    {
        var payable = await _payableRepository.GetByIdAsync(id);
        if (payable == null)
        {
            throw new NotFoundException($"Conta a pagar com ID {id} não foi encontrada");
        }

        // Validar se a conta pode ser cancelada
        if (payable.Status == PayableStatus.Paid)
        {
            throw new BusinessException("Não é possível cancelar uma conta que já foi paga");
        }

        if (payable.Status == PayableStatus.Canceled)
        {
            throw new BusinessException("Esta conta já está cancelada");
        }

        payable.Status = PayableStatus.Canceled;

        await _payableRepository.UpdateAsync(payable);
        return MapToResponse(payable);
    }

    private static PayableResponse MapToResponse(Payable payable)
    {
        return new PayableResponse
        {
            Id = payable.Id,
            SupplierId = payable.SupplierId,
            Description = payable.Description,
            DueDate = payable.DueDate,
            Amount = payable.Amount,
            Status = payable.Status.ToString(),
            PaymentDate = payable.PaymentDate,
            CreatedAt = payable.CreatedAt
        };
    }
}

