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
    public class CreateOrganizationService : ICreateOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public CreateOrganizationService(IOrganizationRepository organizationRepository, IStringLocalizer<Messages> localizer)
        {
            _organizationRepository = organizationRepository;
            _localizer = localizer;
        }

        public async Task<int> ExecuteAsync(Organization organization, CancellationToken cancellationToken = default)
        {
            if(await _organizationRepository.ExistsByRazaoSocialAsync(organization.RazaoSocial, cancellationToken))
                throw new InvalidOperationException(_localizer.For(OrganizationMessage.AlreadyExistsByRazaoSocial));

            if(await _organizationRepository.ExistsByCnpjAsync(organization.Cnpj, cancellationToken))
                throw new InvalidOperationException(_localizer.For(OrganizationMessage.AlreadyExistsByCnpj, organization.Cnpj));

            // Garantir que a organização esteja ativa ao ser criada
            organization.Activate();

            await _organizationRepository.AddAsync(organization);
            await _organizationRepository.SaveChangesAsync(cancellationToken);

            return organization.Id;
        }
    }
}
