using BMBAssessment.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BMBAssessment.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> Set;
    public GenericRepository(AppDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }
    public virtual async Task<T?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await Set.FindAsync(new object[] { id }, cancellationToken);
    }
    public virtual async Task<IReadOnlyCollection<T>> GetAll(CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking().ToListAsync(cancellationToken);
    }
    public virtual Task Add(T entity, CancellationToken cancellationToken = default)
    {
        return Set.AddAsync(entity, cancellationToken).AsTask();
    }

    public virtual void Update(T entity)
    {
        Set.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        Set.Remove(entity);
    }

}
