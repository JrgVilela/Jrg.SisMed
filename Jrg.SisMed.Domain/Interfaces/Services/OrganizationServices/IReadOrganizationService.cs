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
        Task<bool> ExistsByIdAsync(Guid id);
        Task<IEnumerable<Organization>> GetAllAsync();
        Task<Organization?> GetByIdAsync(Guid id);
        Task<Organization?> GetByCnpjAsync(string cnpj);
    }
}
