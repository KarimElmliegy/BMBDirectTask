using BMBAssessment.Application.Interfaces.Repositories;
using BMBAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BMBAssessment.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }
    public async Task<IReadOnlyCollection<Order>> GetByCustomerId(int customerId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking().Include(x => x.Items).Where(x => x.CustomerId == customerId).OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
    }
    public Task<Order?> GetCustomerOrder(int orderId, int customerId, CancellationToken cancellationToken = default)
    {
        return Set.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == orderId && x.CustomerId == customerId, cancellationToken);
    }
    public Task<OrderItem?> GetCustomerOrderItem(int orderId, int itemId, int customerId, CancellationToken cancellationToken = default)
    {
        return Context.OrderItems.Include(x => x.ProductVariant).SingleOrDefaultAsync(x => x.Id == itemId && x.OrderId == orderId && x.Order.CustomerId == customerId, cancellationToken);
    }
    public async Task<IReadOnlyCollection<Order>> GetDeletedBefore(DateTime cutoff, int batchSize, CancellationToken cancellationToken = default)
    {
        return await Set.IgnoreQueryFilters().Where(x => x.DeletedAt < cutoff).OrderBy(x => x.DeletedAt).Take(batchSize).ToListAsync(cancellationToken);
    }
    public void SetOriginalVersion(OrderItem item, byte[] version)
    {
        Context.Entry(item).Property(x => x.Version).OriginalValue = version;
    }
}
