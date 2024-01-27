using Register.Core.Entities;

namespace Register.Application.DTOs
{
    public record CreateOrderDto(decimal TotalPrice, DateTime CreatedAt, Customer Customer, Payment Payment, List<Product> Products, string Reason, StatusOrder Status, bool Cancelled)
    {
        public static implicit operator Order(CreateOrderDto dto)
           => new Order
           {
               TotalPrice = dto.TotalPrice,
               CreatedAt = dto.CreatedAt,
               Customers = dto.Customer,
               Payments = dto.Payment,
               Products = dto.Products,
               Reason = dto.Reason,
               Status = dto.Status,
               Cancelled = dto.Cancelled
             
           };
    }
}
