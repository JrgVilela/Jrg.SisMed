using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices
{
    public interface IDeleteOrganizationService
    {
        Task ExecuteAsync(int id);
    }
}
