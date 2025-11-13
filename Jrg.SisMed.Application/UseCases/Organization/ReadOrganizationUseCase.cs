using Jrg.SisMed.Application.DTOs.OrganizationDto;
using Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.UseCases.Organization
{
    public class ReadOrganizationUseCase
    {
        private IReadOrganizationService _readOrganizationService;

        public ReadOrganizationUseCase(IReadOrganizationService readOrganizationService)
        {
            _readOrganizationService = readOrganizationService;
        }

        public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _readOrganizationService.ExistsByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<ReadOrganizationDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _readOrganizationService.GetAllAsync(cancellationToken);

            if(result.Any() is false)
                return new List<ReadOrganizationDto>(); //Retorna lista vazia

            return result.Select(o => ReadOrganizationDto.FromDomainOrganization(o));
        }

        public async Task<ReadOrganizationDto?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            var result = await _readOrganizationService.GetByCnpjAsync(cnpj, cancellationToken);

            if (result is null)
                return null;

            return ReadOrganizationDto.FromDomainOrganization(result);
        }

        public async Task<ReadOrganizationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await _readOrganizationService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return null;

            return ReadOrganizationDto.FromDomainOrganization(result);
        }
    }
}
