using BMBAssessment.Application.Interfaces.Repositories;

namespace BMBAssessment.Application.Interfaces;
public interface IUnitOfWork
{
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }
    IOrderDeletionRepository OrderDeletions { get; }
    IProductRepository Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
