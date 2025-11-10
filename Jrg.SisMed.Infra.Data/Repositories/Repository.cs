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
    /// <summary>
    /// Implementação genérica do padrão Repository para acesso a dados.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade que herda de Entity.</typeparam>
    public abstract class Repository<T> : IRepository<T> where T : Entity 
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Construtor do repositório.
        /// </summary>
        /// <param name="context">Contexto do banco de dados.</param>
        protected Repository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Obtém uma entidade pelo ID.
        /// </summary>
        /// <param name="id">ID da entidade.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Entidade encontrada ou null.</returns>
        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>
        /// Obtém todas as entidades.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista de todas as entidades.</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Busca entidades que correspondem ao predicado.
        /// </summary>
        /// <param name="predicate">Expressão de filtro.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista de entidades que correspondem ao filtro.</returns>
        public virtual async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Adiciona uma nova entidade ao contexto.
        /// </summary>
        /// <param name="entity">Entidade a ser adicionada.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <remarks>
        /// A entidade não é salva no banco até que SaveChangesAsync seja chamado.
        /// </remarks>
        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.SetCreatedAt();

            await _dbSet.AddAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Marca uma entidade para atualização.
        /// </summary>
        /// <param name="entity">Entidade a ser atualizada.</param>
        /// <remarks>
        /// A entidade não é salva no banco até que SaveChangesAsync seja chamado.
        /// Este método é síncrono seguindo o padrão do EF Core.
        /// </remarks>
        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.SetUpdatedAt();

            _dbSet.Update(entity);
        }

        /// <summary>
        /// Marca uma entidade para remoção.
        /// </summary>
        /// <param name="entity">Entidade a ser removida.</param>
        /// <remarks>
        /// A entidade não é removida do banco até que SaveChangesAsync seja chamado.
        /// Este método é síncrono seguindo o padrão do EF Core.
        /// </remarks>
        public virtual void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Retorna um IQueryable para construir queries complexas.
        /// </summary>
        /// <returns>IQueryable da entidade.</returns>
        /// <remarks>
        /// Use este método para criar queries complexas com Include, OrderBy, etc.
        /// </remarks>
        public virtual IQueryable<T> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        /// Verifica se existe uma entidade que corresponde ao predicado.
        /// </summary>
        /// <param name="predicate">Expressão de filtro.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>True se existe, false caso contrário.</returns>
        public virtual async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Obtém a primeira entidade que corresponde ao predicado ou null.
        /// </summary>
        /// <param name="predicate">Expressão de filtro.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Primeira entidade encontrada ou null.</returns>
        public virtual async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Conta o número de entidades que correspondem ao predicado.
        /// </summary>
        /// <param name="predicate">Expressão de filtro (opcional).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Número de entidades.</returns>
        public virtual async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null, 
            CancellationToken cancellationToken = default)
        {
            return predicate == null 
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Adiciona múltiplas entidades ao contexto.
        /// </summary>
        /// <param name="entities">Coleção de entidades a serem adicionadas.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        public virtual async Task AddRangeAsync(
            IEnumerable<T> entities, 
            CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Remove múltiplas entidades do contexto.
        /// </summary>
        /// <param name="entities">Coleção de entidades a serem removidas.</param>
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.RemoveRange(entities);
        }

        /// <summary>
        /// Salva todas as alterações pendentes no banco de dados.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Número de registros afetados.</returns>
        /// <exception cref="DbUpdateException">Lançado quando há erro ao salvar no banco.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Lançado quando há conflito de concorrência.</exception>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                // Log do erro (adicionar logging futuramente)
                throw new InvalidOperationException("Erro ao salvar alterações no banco de dados.", ex);
            }
        }

        /// <summary>
        /// Libera os recursos utilizados pelo contexto.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera os recursos utilizados pelo contexto de forma assíncrona.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }
    }
}
