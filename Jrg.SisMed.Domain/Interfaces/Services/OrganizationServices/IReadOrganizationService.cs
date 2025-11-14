using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices
{
    public interface IReadOrganizationService
    {
        Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Organization>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Organization?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Organization?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    }
}
