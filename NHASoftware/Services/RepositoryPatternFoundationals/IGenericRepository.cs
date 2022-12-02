using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace NHASoftware.Services.RepositoryPatternFoundationals
{
    public interface IGenericRepository<T> where T : class
    {
        public void Add(T entity);
        public void AddRange(IEnumerable<T> entities);
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int? id);
        public void Remove(T entity);
        public void RemoveRange(IEnumerable<T> entities);
        public EntityEntry<T> Update(T entity);
    }
}
