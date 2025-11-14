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
    public class UpdateOrganizationUseCase
    {
        private readonly IUpdateOrganizationService _updateOrganizationService;
        private IStringLocalizer<Messages> _localizer;

        public UpdateOrganizationUseCase(IUpdateOrganizationService updateOrganizationService, IStringLocalizer<Messages> localizer)
        {
            _updateOrganizationService = updateOrganizationService;
            _localizer = localizer;
        }

        public async Task ExecuteAsync(int id, UpdateOrganizationDto organizationDto, CancellationToken cancellationToken = default)
        {
            if(id <=0 || organizationDto == null)
                throw new ArgumentNullException(nameof(organizationDto), _localizer.For(CommonMessage.ArgumentNull));

            var organization = organizationDto.ToDomainOrganization();

            await _updateOrganizationService.ExecuteAsync(id, organization, cancellationToken);
        }
    }
}