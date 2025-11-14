using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Repositories
{
    public interface IOrganizationRepository : IRepository<Organization>
    {
        Task<bool> ExistsByRazaoSocialAsync(string razaoSocial, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    }
}
