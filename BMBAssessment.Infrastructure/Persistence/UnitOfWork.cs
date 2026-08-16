using BMBAssessment.Application.Interfaces;
using BMBAssessment.Application.Interfaces.Repositories;

namespace BMBAssessment.Infrastructure.Persistence;
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public ICustomerRepository Customers { get; }
    public IOrderRepository Orders { get; }
    public IOrderDeletionRepository OrderDeletions { get; }
    public IProductRepository Products { get; }

    public UnitOfWork(AppDbContext context, ICustomerRepository customers, IOrderRepository orders, IOrderDeletionRepository orderDeletions, IProductRepository products) 
    { 
        _context = context; 
        Customers = customers; 
        Orders = orders; 
        OrderDeletions = orderDeletions; 
        Products = products;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
