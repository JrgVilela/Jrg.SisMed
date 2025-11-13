using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices;
using Jrg.SisMed.Domain.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Services.OrganizationServices
{
    public class DeleteOrganizationService : IDeleteOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public DeleteOrganizationService(IOrganizationRepository organizationRepository, IStringLocalizer<Messages> localizer)
        {
            _organizationRepository = organizationRepository;
            _localizer = localizer;
        }

        public async Task ExecuteAsync(int id)
        {
            var organization = await _organizationRepository.GetByIdAsync(id);
            if (organization == null)
                throw new KeyNotFoundException(_localizer.For(OrganizationMessage.NotFound));

            await _organizationRepository.RemoveAsync(id);
            await _organizationRepository.SaveChangesAsync();
        }
    }
}
