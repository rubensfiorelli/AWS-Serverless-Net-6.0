using Amazon.DynamoDBv2.DataModel;
using Register.Core.Common;

namespace Register.Core.Entities
{
    [DynamoDBTable("Customers")]
    public sealed class Customer : BaseEntity
    {

        public string Cpf { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;

    }
}
