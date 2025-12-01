using ContasAPagar.API.DTOs.Payable;
using ContasAPagar.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContasAPagar.API.Repositories;

public class PayableRepository : IPayableRepository
{
    private readonly IMongoCollection<Payable> _payables;

    public PayableRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _payables = database.GetCollection<Payable>("payables");

        // Criar índices para melhorar performance de consultas
        var indexKeys = Builders<Payable>.IndexKeys
            .Ascending(p => p.SupplierId)
            .Ascending(p => p.DueDate)
            .Ascending(p => p.Status);
        _payables.Indexes.CreateOneAsync(new CreateIndexModel<Payable>(indexKeys));
    }

    public async Task<Payable> CreateAsync(Payable payable)
    {
        await _payables.InsertOneAsync(payable);
        return payable;
    }

    public async Task<Payable?> GetByIdAsync(string id)
    {
        return await _payables.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<(List<Payable> Items, int TotalCount)> GetFilteredAsync(PayableFilterRequest filter)
    {
        var filterBuilder = Builders<Payable>.Filter;
        var filters = new List<FilterDefinition<Payable>>();

        // Filtro por SupplierId
        if (!string.IsNullOrEmpty(filter.SupplierId))
        {
            filters.Add(filterBuilder.Eq(p => p.SupplierId, filter.SupplierId));
        }

        // Filtro por período de vencimento
        if (filter.StartDueDate.HasValue)
        {
            filters.Add(filterBuilder.Gte(p => p.DueDate, filter.StartDueDate.Value));
        }

        if (filter.EndDueDate.HasValue)
        {
            filters.Add(filterBuilder.Lte(p => p.DueDate, filter.EndDueDate.Value));
        }

        // Filtro por Status
        if (!string.IsNullOrEmpty(filter.Status) && 
            Enum.TryParse<PayableStatus>(filter.Status, true, out var status))
        {
            filters.Add(filterBuilder.Eq(p => p.Status, status));
        }

        var combinedFilter = filters.Any() 
            ? filterBuilder.And(filters) 
            : filterBuilder.Empty;

        // Contagem total
        var totalCount = await _payables.CountDocumentsAsync(combinedFilter);

        // Paginação
        var skip = (filter.Page - 1) * filter.PageSize;
        var items = await _payables
            .Find(combinedFilter)
            .SortByDescending(p => p.DueDate)
            .Skip(skip)
            .Limit(filter.PageSize)
            .ToListAsync();

        return (items, (int)totalCount);
    }

    public async Task<bool> UpdateAsync(Payable payable)
    {
        var result = await _payables.ReplaceOneAsync(
            p => p.Id == payable.Id,
            payable
        );

        return result.ModifiedCount > 0;
    }
}

