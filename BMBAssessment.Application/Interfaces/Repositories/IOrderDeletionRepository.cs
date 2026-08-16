using BMBAssessment.Domain.Entities;

namespace BMBAssessment.Application.Interfaces.Repositories;
public interface IOrderDeletionRepository : IGenericRepository<OrderDeletion>
{
    Task<int> CountCustomerDeletionsOnDateAsync(int customerId, DateTime utcDate, CancellationToken cancellationToken = default);
}
