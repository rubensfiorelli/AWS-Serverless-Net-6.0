using Register.Application.DTOs;
using Register.Core.Contracts;

namespace Register.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository) => _repository = repository;
        
        public async Task<string> Add(CreateOrderDto model)
        {
            await _repository.AddAsync(model);

            return model.Status.ToString();
        }
    }
}
