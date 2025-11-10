using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.Data.Repositories
{
    internal class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        private readonly ApplicationDbContext _context;
        public OrganizationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            return await _context.Organizations.AnyAsync(o => o.Cnpj == cnpj, cancellationToken);
        }

        public async Task<bool> ExistsByRazaoSocialAsync(string razaoSocial, CancellationToken cancellationToken = default)
        {
            return await _context.Organizations.AnyAsync(o => o.RazaoSocial == razaoSocial, cancellationToken);
        }
    }
}
