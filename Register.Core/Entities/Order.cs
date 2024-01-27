using Amazon.DynamoDBv2.DataModel;
using Register.Core.Common;

namespace Register.Core.Entities
{
    public enum StatusOrder
    {
        Collected,
        Paid,
        Invoiced,
        Reserved

    }

    [DynamoDBTable("Orders")]
    public sealed class Order : BaseEntity
    {
        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public List<Product> Products { get; set; }

        public Customer? Customers { get; init; }

        public Payment? Payments { get; init; }

        public string? Reason { get; set; }

        public StatusOrder Status { get; set; } = StatusOrder.Collected;

        public bool Cancelled { get; set; } = false;
    }
}
