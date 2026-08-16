using BMBAssessment.Domain.Entities;

namespace BMBAssessment.Application.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IReadOnlyCollection<Product>> GetActiveProducts(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProductVariant>> GetActiveVariants(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProductVariant>> GetVariants(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
