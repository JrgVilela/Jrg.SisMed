using FluentValidation;
using Jrg.SisMed.Application.DTOs.ProfessionalDto;
using Jrg.SisMed.Application.Validations.ProfessionalValidations;
using Jrg.SisMed.Domain.Interfaces.Providers.ProfessionalProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.UseCases.ProfessionalUseCases
{
    /// <summary>
    /// Use case responsável pelo registro de profissionais no sistema.
    /// Realiza a validação dos dados e delega a criação para a factory apropriada.
    /// </summary>
    public class RegisterProfessionalUseCase
    {
        private readonly IProfessionalFactoryProvider _provider;
        private readonly IValidator<RegisterDto> _validator;

        public RegisterProfessionalUseCase(
            IProfessionalFactoryProvider provider,
            IValidator<RegisterDto> validator)
        {
            _provider = provider;
            _validator = validator;
        }

        /// <summary>
        /// Executa o registro de um profissional.
        /// </summary>
        /// <param name="dto">DTO com os dados do profissional a ser registrado.</param>
        /// <returns>ID do profissional criado.</returns>
        /// <exception cref="ValidationException">Lançada quando os dados são inválidos.</exception>
        public async Task<int> ExecuteAsync(RegisterDto dto, CancellationToken cancellationToken = default)
        {
            // Valida o DTO
            var validationResult = await _validator.ValidateAsync(dto);
            
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Converte DTO para entidade de domínio
            dynamic professional = dto.ToDomain();

            // Obtém a factory apropriada baseada no tipo de profissional
            var factory = _provider.GetFactory(dto.ProfessionalType);
            dynamic service = factory.CreateRegister();

            // Executa o registro
            return await service.ExecuteAsync(professional, cancellationToken);
        }
    }
}
