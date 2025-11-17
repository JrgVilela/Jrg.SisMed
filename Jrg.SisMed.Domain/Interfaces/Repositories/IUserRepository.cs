using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> Login(string email, string password, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Verifica se um email já está cadastrado.
        /// </summary>
        /// <param name="email">Email a ser verificado.</param>
        /// <param name="excludeUserId">ID do usuário a ser excluído da verificação (para updates).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>True se o email já existe, false caso contrário.</returns>
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default);
    } 
}