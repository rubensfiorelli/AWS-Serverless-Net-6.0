using Amazon.DynamoDBv2.DataModel;

namespace Register.Core.Common
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
           => Id = Guid.NewGuid().ToString();

        [DynamoDBHashKey]
        public string Id { get; protected init; }

        public bool Equals(string id)
            => Id == id;

        public override int GetHashCode()
            => base.GetHashCode();
    }
}
