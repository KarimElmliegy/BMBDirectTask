using BMBAssessment.Application.Interfaces.Repositories;
using BMBAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BMBAssessment.Infrastructure.Persistence.Repositories;
public sealed class OrderDeletionRepository : GenericRepository<OrderDeletion>, IOrderDeletionRepository
{
    public OrderDeletionRepository(AppDbContext context) : base(context) 
    {
        
    }
    public Task<int> CountCustomerDeletionsOnDateAsync(int customerId, DateTime utcDate, CancellationToken cancellationToken = default)
    {
        var date = utcDate.Date;
        var nextDate = date.AddDays(1);
        
        return Set.CountAsync(x => x.CustomerId == customerId
            && x.DeletedAt >= date && x.DeletedAt < nextDate
            && x.OrderCreatedAt >= date && x.OrderCreatedAt < nextDate, cancellationToken);
    }
}
