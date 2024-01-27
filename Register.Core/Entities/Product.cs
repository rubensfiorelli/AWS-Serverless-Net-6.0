namespace Register.Core.Entities
{
    public class Product
    {
        public string? Id { get; set; }
        public string? Title { get; set; }

        public decimal? Price { get; set; } = 0;

        public byte Qty { get; set; }

        public bool Reserved { get; set; } = false;
    }
}