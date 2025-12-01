using ContasAPagar.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContasAPagar.API.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly IMongoCollection<Supplier> _suppliers;

    public SupplierRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _suppliers = database.GetCollection<Supplier>("suppliers");

        // Criar índice único para o documento
        var indexKeys = Builders<Supplier>.IndexKeys.Ascending(s => s.Document);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<Supplier>(indexKeys, indexOptions);
        _suppliers.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        await _suppliers.InsertOneAsync(supplier);
        return supplier;
    }

    public async Task<Supplier?> GetByIdAsync(string id)
    {
        return await _suppliers.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Supplier?> GetByDocumentAsync(string document)
    {
        return await _suppliers.Find(s => s.Document == document).FirstOrDefaultAsync();
    }

    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _suppliers.Find(_ => true).ToListAsync();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _suppliers.Find(s => s.Id == id).AnyAsync();
    }
}

