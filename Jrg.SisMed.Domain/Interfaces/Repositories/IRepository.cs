using Jrg.SisMed.Domain.Entities;
using System.Linq.Expressions;

namespace Jrg.SisMed.Domain.Interfaces.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task CommitAsync();
    }
}