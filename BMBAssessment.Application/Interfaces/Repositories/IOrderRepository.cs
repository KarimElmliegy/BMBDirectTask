using BMBAssessment.Domain.Entities;

namespace BMBAssessment.Application.Interfaces.Repositories;
public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IReadOnlyCollection<Order>> GetByCustomerId(int customerId, CancellationToken cancellationToken = default);
    Task<Order?> GetCustomerOrder(int orderId, int customerId, CancellationToken cancellationToken = default);
    Task<OrderItem?> GetCustomerOrderItem(int orderId, int itemId, int customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> GetDeletedBefore(DateTime cutoff, int batchSize, CancellationToken cancellationToken = default);
    void SetOriginalVersion(OrderItem item, byte[] version);
}
