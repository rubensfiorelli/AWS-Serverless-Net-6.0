using Register.Core.Common;

namespace Register.Core.Entities
{
    public class Payment : BaseEntity
    {
        public string? CardNumber { get; init; }

        public string? Validate { get; init; }

        public string? CVV { get; init; }
    }
}
