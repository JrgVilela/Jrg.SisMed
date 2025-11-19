using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
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
    public class ReadOrganizationService : IReadOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public ReadOrganizationService(IOrganizationRepository organizationRepository, IStringLocalizer<Messages> localizer)
        {
            _organizationRepository = organizationRepository;
            _localizer = localizer;
        }

        public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _organizationRepository.ExistByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Organization>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return  await _organizationRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Organization?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            if(cnpj.IsNullOrEmpty())
                throw new ArgumentException(_localizer.For(OrganizationMessage.CnpjRequired), nameof(cnpj));

            if(!cnpj.IsCnpj())
                throw new ArgumentException(_localizer.For(OrganizationMessage.CnpjInvalid, cnpj), nameof(cnpj));

            var normalizedCnpj = cnpj.GetOnlyNumbers();

            if(!(await _organizationRepository.ExistsByCnpjAsync(normalizedCnpj, cancellationToken)))
                throw new NotFoundException("Organization", normalizedCnpj);

            var result = await _organizationRepository.FindAsync(o => o.Cnpj.Equals(normalizedCnpj), cancellationToken);

            if(result.Count() > 1)
                throw new InvalidOperationException(_localizer.For(OrganizationMessage.NotFound, cnpj));

            return result.FirstOrDefault();
        }

        public async Task<Organization?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var organization = await _organizationRepository.GetByIdAsync(id, cancellationToken);
            
            if (organization == null)
                throw new NotFoundException("Organization", id);

            return organization;
        }
    }
}
