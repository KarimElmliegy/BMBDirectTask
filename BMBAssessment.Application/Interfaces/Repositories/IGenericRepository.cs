namespace BMBAssessment.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetById(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<T>> GetAll(CancellationToken cancellationToken = default);
    Task Add(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
