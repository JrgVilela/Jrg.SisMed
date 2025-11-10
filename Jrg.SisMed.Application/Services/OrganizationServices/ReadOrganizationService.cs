using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Services.OrganizationServices
{
    public class ReadOrganizationService : IReadOrganizationService
    {
        public Task<bool> ExistsByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Organization>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Organization?> GetByCnpjAsync(string cnpj)
        {
            throw new NotImplementedException();
        }

        public Task<Organization?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
