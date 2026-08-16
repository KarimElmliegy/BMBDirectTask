using BMBAssessment.Application.Interfaces.Repositories;
using BMBAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BMBAssessment.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyCollection<Product>> GetActiveProducts(CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking().Where(x => x.IsActive)
            .Include(x => x.Variants.Where(variant => variant.IsActive))
            .OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProductVariant>> GetActiveVariants(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var variantIds = ids.ToArray();
        return await Context.ProductVariants.Include(x => x.Product)
            .Where(x => variantIds.Contains(x.Id) && x.IsActive && x.Product.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProductVariant>> GetVariants(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var variantIds = ids.ToArray();
        return await Context.ProductVariants.Where(x => variantIds.Contains(x.Id)).ToListAsync(cancellationToken);
    }
}
