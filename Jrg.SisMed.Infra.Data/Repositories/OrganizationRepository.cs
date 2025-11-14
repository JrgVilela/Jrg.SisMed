using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Resources;
using Jrg.SisMed.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.Data.Repositories
{
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(ApplicationDbContext context, IStringLocalizer<Messages> localizer) : base(context, localizer)
        {
        }

        public override async Task<Organization?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // EU quero trazer a organização e caso tenho um telefone marcado como principal, eu quero trazê-lo também
            return await _dbSet
                .Include(o => o.Phones.Where(p => p.IsPrincipal))
                .ThenInclude(op => op.Phone)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public override async Task<IEnumerable<Organization>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // EU quero trazer todas as organizações e caso tenho um telefone marcado como principal, eu quero trazê-lo também
            return await _dbSet
                .Include(o => o.Phones.Where(p => p.IsPrincipal))
                .ThenInclude(op => op.Phone)
                .ToListAsync(cancellationToken);
        }

        public override async Task<IEnumerable<Organization>> FindAsync(Expression<Func<Organization, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(o => o.Phones.Where(p => p.IsPrincipal))
                .ThenInclude(op => op.Phone)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(o => o.Cnpj == cnpj, cancellationToken);
        }

        public async Task<bool> ExistsByRazaoSocialAsync(string razaoSocial, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(o => o.RazaoSocial == razaoSocial, cancellationToken);
        }
    }
}
