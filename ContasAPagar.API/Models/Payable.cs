using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContasAPagar.API.Models;

public class Payable
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("supplierId")]
    [BsonRequired]
    public string SupplierId { get; set; } = string.Empty;

    [BsonElement("description")]
    [BsonRequired]
    public string Description { get; set; } = string.Empty;

    [BsonElement("dueDate")]
    [BsonRequired]
    public DateTime DueDate { get; set; }

    [BsonElement("amount")]
    [BsonRequired]
    public decimal Amount { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public PayableStatus Status { get; set; } = PayableStatus.Pending;

    [BsonElement("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum PayableStatus
{
    Pending,
    Paid,
    Canceled
}

