using System.Linq.Expressions;

namespace Jrg.SisMed.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface genérica para repositórios de acesso a dados.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade.</typeparam>
    public interface IRepository<T> : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Obtém uma entidade pelo ID.
        /// </summary>
        /// <param name="id">ID da entidade.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Entidade encontrada ou null.</returns>
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém todas as entidades.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista de todas as entidades.</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca entidades que correspondem ao predicado.
        /// </summary>
        /// <param name="predicate">Expressão de filtro.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista de entidades que correspondem ao filtro.</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adiciona uma nova entidade ao contexto.
        /// </summary>
        /// <param name="entity">Entidade a ser adicionada.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marca uma entidade para atualização.
        /// </summary>
        /// <param name="entity">Entidade a ser atualizada.</param>
        void Update(T entity);

        /// <summary>
        /// Marca uma entidade para remoção.
        /// </summary>
        /// <param name="entity">Entidade a ser removida.</param>
        void Remove(T entity);

        /// <summary>
        /// Retorna um IQueryable para construir queries complexas.
        /// </summary>
        /// <returns>IQueryable da entidade.</returns>
        IQueryable<T> AsQueryable();

        /// <summary>
        /// Salva todas as alterações pendentes no banco de dados.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Número de registros afetados.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}