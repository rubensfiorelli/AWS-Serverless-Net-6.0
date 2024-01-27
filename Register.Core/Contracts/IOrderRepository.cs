using Register.Core.Entities;

namespace Register.Core.Contracts
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);

    }
}
