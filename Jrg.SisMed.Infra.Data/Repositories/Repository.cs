using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : Entity 
    {
        private readonly ApplicationDbContext _context;

        protected Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().Where(predicate).ToListAsync();

        public Task CreateAsync(T entity)
        {
            
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task CommitAsync()
        {
            throw new NotImplementedException();
        }
    }
}
