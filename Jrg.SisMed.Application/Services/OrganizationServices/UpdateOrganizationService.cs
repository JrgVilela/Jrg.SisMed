using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Services.OrganizationServices
{
    public class UpdateOrganizationService : IUpdateOrganizationService
    {
        public Task Execute(Guid id, Organization organization)
        {
            throw new NotImplementedException();
        }
    }
}
