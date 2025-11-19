using Jrg.SisMed.Application.DTOs.OrganizationDto;
using Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices;
using Jrg.SisMed.Domain.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.UseCases.Organization
{
    public class CreateOrganizationUseCase
    {
        private readonly ICreateOrganizationService _createOrganizationService;
        private readonly IStringLocalizer<Messages> _localizer;

        public CreateOrganizationUseCase(ICreateOrganizationService createOrganizationService, IStringLocalizer<Messages> localizer)
        {
            _createOrganizationService = createOrganizationService;
            _localizer = localizer;
        }

        public virtual async Task<int> ExecuteAsync(CreateOrganizationDto organizationDto, CancellationToken cancellationToken = default)
        {
            if (organizationDto == null)
                throw new ArgumentNullException(nameof(organizationDto), _localizer.For(CommonMessage.ArgumentNull_Generic, nameof(organizationDto)));

            var organization = organizationDto.ToDomainOrganization();
            return await _createOrganizationService.ExecuteAsync(organization, cancellationToken);
        }
    }
}
