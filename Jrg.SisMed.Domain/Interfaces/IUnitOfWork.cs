using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces
{
    /// <summary>
    /// Interface do padrão Unit of Work para gerenciar transações.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Salva todas as alterações pendentes no banco de dados.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Número de registros afetados.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
