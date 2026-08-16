using BMBAssessment.Application.Interfaces.Repositories;
using BMBAssessment.Domain.Entities;

namespace BMBAssessment.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository : GenericRepository<ApplicationUser>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) 
    {
    }
}
