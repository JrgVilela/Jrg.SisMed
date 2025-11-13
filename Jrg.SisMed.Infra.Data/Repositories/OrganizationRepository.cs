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
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(ApplicationDbContext context, IStringLocalizer<Messages> localizer) : base(context, localizer)
        {
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
