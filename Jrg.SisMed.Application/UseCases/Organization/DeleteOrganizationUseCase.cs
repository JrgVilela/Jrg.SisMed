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
    public class DeleteOrganizationUseCase
    {
        private readonly IDeleteOrganizationService _deleteOrganizationService;
        private readonly IStringLocalizer<Messages> _localizer;

        public DeleteOrganizationUseCase(IDeleteOrganizationService deleteOrganizationService, IStringLocalizer<Messages> localizer)
        {
            _deleteOrganizationService = deleteOrganizationService;
            _localizer = localizer;
        }

        public virtual async Task ExecuteAsync(int id)
        {
            if(id <= 0)
                throw new ArgumentException(_localizer.For(CommonMessage.InvalidArgument), nameof(id));

            await _deleteOrganizationService.ExecuteAsync(id);
        }
    }
}
