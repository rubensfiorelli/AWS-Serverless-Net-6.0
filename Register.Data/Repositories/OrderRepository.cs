using Amazon.DynamoDBv2.DataModel;
using Register.Core.Contracts;
using Register.Core.Entities;

namespace Register.Data.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly IDynamoDBContext _context;

        public OrderRepository(IDynamoDBContext context) => _context = context;


        public async Task AddAsync(Order order)
        {
            await _context.LoadAsync<Order>(order.Id);

            await _context.SaveAsync(order);
        }
    }
}
