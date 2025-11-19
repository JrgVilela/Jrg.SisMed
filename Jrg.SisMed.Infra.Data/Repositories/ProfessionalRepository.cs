using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Resources;
using Jrg.SisMed.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.Data.Repositories
{
    /// <summary>
    /// Implementação do repositório de Professional (User).
    /// </summary>
    public class ProfessionalRepository : Repository<Professional>, IProfessionalRepository
    {
        /// <summary>
        /// Construtor do repositório de usuários.
        /// </summary>
        /// <param name="context">Contexto do banco de dados.</param>
        public ProfessionalRepository(ApplicationDbContext context, IStringLocalizer<Messages> localizer) : base(context, localizer)
        {
        }

        /// <summary>
        /// Obtém um usuário pelo CPF.
        /// </summary>
        /// <param name="cpf">CPF do usuário (apenas números).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Usuário encontrado ou null.</returns>
        public async Task<Professional?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Cpf == cpf, cancellationToken);
        }

        /// <summary>
        /// Verifica se um CPF já está cadastrado.
        /// </summary>
        /// <param name="cpf">CPF a ser verificado (apenas números).</param>
        /// <param name="excludeUserId">ID do usuário a ser excluído da verificação (para updates).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>True se o CPF já existe, false caso contrário.</returns>
        public async Task<bool> CpfExistsAsync(
            string cpf, 
            int? excludeUserId = null, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            var query = _dbSet.Where(u => u.Cpf == cpf);

            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Obtém usuários ativos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista de usuários ativos.</returns>
        public async Task<IEnumerable<Professional>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.State == ProfessionalEnum.State.Active)
                .OrderBy(u => u.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtém um usuário com seus endereços e telefones (eager loading).
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Usuário com relacionamentos carregados ou null.</returns>
        public async Task<Professional?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.Addresses)
                    .ThenInclude(pa => pa.Address)
                .Include(u => u.Phones)
                    .ThenInclude(pp => pp.Phone)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
    }
}
