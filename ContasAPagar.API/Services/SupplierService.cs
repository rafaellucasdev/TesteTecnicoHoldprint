using ContasAPagar.API.DTOs.Supplier;
using ContasAPagar.API.Exceptions;
using ContasAPagar.API.Models;
using ContasAPagar.API.Repositories;

namespace ContasAPagar.API.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierResponse> CreateAsync(CreateSupplierRequest request)
    {
        // Verificar se já existe um fornecedor com o mesmo documento
        var existingSupplier = await _supplierRepository.GetByDocumentAsync(request.Document);
        if (existingSupplier != null)
        {
            throw new BusinessException($"Já existe um fornecedor cadastrado com o documento {request.Document}");
        }

        var supplier = new Supplier
        {
            Name = request.Name,
            Document = request.Document,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _supplierRepository.CreateAsync(supplier);
        return MapToResponse(created);
    }

    public async Task<SupplierResponse?> GetByIdAsync(string id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        return supplier != null ? MapToResponse(supplier) : null;
    }

    public async Task<List<SupplierResponse>> GetAllAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(MapToResponse).ToList();
    }

    private static SupplierResponse MapToResponse(Supplier supplier)
    {
        return new SupplierResponse
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Document = supplier.Document,
            Email = supplier.Email,
            CreatedAt = supplier.CreatedAt
        };
    }
}

