using Jrg.SisMed.Application.DTOs.ProfessionalDto;
using Jrg.SisMed.Domain.Interfaces.Providers.ProfessionalProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.UseCases.ProfessionalUseCases
{
    public class RegisterProfessionalUseCase
    {
        private readonly IProfessionalFactoryProvider _provider;

        public RegisterProfessionalUseCase(IProfessionalFactoryProvider provider)
        {
            _provider = provider;
        }

        public async Task<int> ExecuteAsync(RegisterDto dto)
        {
            var professional = dto.ToDomain();

            dynamic factory = _provider.GetFactory(dto.ProfessionalType);
            dynamic service = factory.CreateRegister();

            return await service.ExecuteAsync(professional);
        }
    }
}
