using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices
{
    public interface IPhoneOrganizationService
    {
        public Task AddPhoneAsync(Guid organizationId, string phoneNumber);
        public Task RemovePhoneAsync(Guid organizationId, Guid phoneId);
    }
}