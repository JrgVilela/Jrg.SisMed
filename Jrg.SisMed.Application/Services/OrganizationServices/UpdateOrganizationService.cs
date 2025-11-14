using Jrg.SisMed.Domain.Entities;
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
    public class UpdateOrganizationService : IUpdateOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateOrganizationService(IOrganizationRepository organizationRepository, IStringLocalizer<Messages> localizer)
        {
            _organizationRepository = organizationRepository;
            _localizer = localizer;
        }

        public async Task ExecuteAsync(int id, Organization organization, CancellationToken cancellationToken = default)
        {
            if (organization == null)
                throw new ArgumentNullException(_localizer.For(CommonMessage.ArgumentNull_Generic));

            if(organization.Id <= 0)
                throw new ArgumentNullException(_localizer.For(CommonMessage.InvalidArgument, nameof(organization.Id)));

            var currentOrganization = await _organizationRepository.GetByIdAsync(id, cancellationToken);
            if(currentOrganization == null)
                throw new KeyNotFoundException(_localizer.For(OrganizationMessage.NotFound));


            currentOrganization.Update(organization.NameFantasia, organization.RazaoSocial, organization.Cnpj, organization.State);

            await _organizationRepository.UpdateAsync(currentOrganization, cancellationToken);
            await _organizationRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
