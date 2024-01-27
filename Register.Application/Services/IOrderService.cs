using Register.Application.DTOs;

namespace Register.Application.Services
{
    public interface IOrderService
    {
        Task<string> Add(CreateOrderDto model);

    }
}
