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
    public class PsychologyRepository : Repository<Psychologist>, IProfessionalRepository<Psychologist>
    {
        public PsychologyRepository(ApplicationDbContext context, IStringLocalizer<Messages> localizer) : base(context, localizer)
        {
        }

        public async Task<bool> CpfExistsAsync(string cpf, int? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            var query = _dbSet.Where(p => p.Cpf == cpf);

            if (excludeUserId.HasValue)
                query = query.Where(p => p.Id != excludeUserId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> RegisterNumberExistsAsync(string registerNumber, int? excludeProfessionalId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(registerNumber))
                return false;

            var query = _dbSet.Where(p => p.Crp == registerNumber);

            if (excludeProfessionalId.HasValue)
                query = query.Where(p => p.Id != excludeProfessionalId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<IEnumerable<Psychologist>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.State == ProfessionalEnum.State.Active)
                .ToListAsync(cancellationToken);
        }

        public async Task<Psychologist?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return null;

            return await _dbSet
                .FirstOrDefaultAsync(p => p.Cpf == cpf, cancellationToken);
        }

        public async Task<Psychologist?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Addresses)
                    .ThenInclude(pa => pa.Address)
                .Include(p => p.Phones)
                    .ThenInclude(pp => pp.Phone)
                .Include(p => p.User)
                .Include(p => p.Organizations)
                    .ThenInclude(op => op.Organization)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
    }
}
