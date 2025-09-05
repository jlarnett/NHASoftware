using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
namespace NHA.Website.Software.Services.RepositoryPatternFoundationals;
public interface IGenericRepository<T> where T : class
{
    public Task AddAsync(T entity);
    public Task AddRange(IEnumerable<T> entities);
    public IEnumerable<T> Find(Expression<Func<T, bool>> expression);
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<T?> GetByIdAsync(int? id);
    public void Remove(T entity);
    public void RemoveRange(IEnumerable<T> entities);
    public EntityEntry<T> Update(T entity);
    public Task<int> CountAsync(Expression<Func<T, bool>> expression);
    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    public Task<IEnumerable<T>> FindWithoutTrackingAsync(Expression<Func<T, bool>> expression);
    public Task<IEnumerable<T>> GetResultPageAsync(int pageNumber = 1, int pageSize = 25);
}
