using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface específica para o repositório de Professional (User).
    /// </summary>
    public interface IProfessionalRepository : IRepository<Professional>
    {
       
        /// <summary>
        /// Obtém um usuário pelo CPF.
        /// </summary>
        /// <param name="cpf">CPF do usuário (apenas números).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Usuário encontrado ou null.</returns>
        Task<Professional?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);


        /// <summary>
        /// Verifica se um CPF já está cadastrado.
        /// </summary>
        /// <param name="cpf">CPF a ser verificado (apenas números).</param>
        /// <param name="excludeUserId">ID do usuário a ser excluído da verificação (para updates).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>True se o CPF já existe, false caso contrário.</returns>
        Task<bool> CpfExistsAsync(string cpf, int? excludeUserId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém usuários ativos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista de usuários ativos.</returns>
        Task<IEnumerable<Professional>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém um usuário com seus endereços e telefones (eager loading).
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Usuário com relacionamentos carregados ou null.</returns>
        Task<Professional?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    }
}
