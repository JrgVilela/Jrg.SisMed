using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Interfaces.Services.ProfessionalServices;
using Jrg.SisMed.Domain.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Services.ProfessionalServices.PsychologyServices
{
    /// <summary>
    /// Serviço responsável pelo registro de profissionais psicólogos.
    /// Valida unicidade de CPF, CRP, Email, CNPJ e Razão Social antes do registro.
    /// </summary>
    public class PsychologyRegisterService : IRegisterService<Psychologist>
    {
        private readonly IProfessionalRepository<Psychologist> _repository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public PsychologyRegisterService(
            IProfessionalRepository<Psychologist> repository, 
            IOrganizationRepository organizationRepository, 
            IUserRepository userRepository,
            IStringLocalizer<Messages> localizer)
        {
            _repository = repository;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _localizer = localizer;
        }

        /// <summary>
        /// Executa o registro de um psicólogo no sistema.
        /// </summary>
        /// <param name="professional">Psicólogo a ser registrado.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>ID do profissional criado.</returns>
        /// <exception cref="ConflictException">Lançada quando CPF, CRP, Email, CNPJ ou Razão Social já existem.</exception>
        public async Task<int> ExecuteAsync(Psychologist professional, CancellationToken cancellationToken = default)
        {
            // Valida unicidade dos dados antes do registro
            await ValidateUniquenessAsync(professional, cancellationToken);

            // Registra o profissional
            await _repository.AddAsync(professional, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return professional.Id;
        }

        /// <summary>
        /// Valida se os dados do profissional são únicos no banco de dados.
        /// </summary>
        private async Task ValidateUniquenessAsync(Psychologist professional, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            // ========================================
            // VALIDAÇÃO DO PROFISSIONAL
            // ========================================

            // Valida CPF único
            var cpfNormalized = professional.Cpf.GetOnlyNumbers();
            if (await _repository.CpfExistsAsync(cpfNormalized, null, cancellationToken))
            {
                errors.Add(_localizer.For(ProfessionalMessage.CpfAlreadyExists).Value);
            }

            // Valida CRP único (Register Number)
            var crpNormalized = professional.Crp.GetOnlyNumbers();
            if (await _repository.RegisterNumberExistsAsync(crpNormalized, null, cancellationToken))
            {
                errors.Add(_localizer.For(ProfessionalMessage.RegisterNumberAlreadyExists).Value);
            }

            // ========================================
            // VALIDAÇÃO DO USUÁRIO
            // ========================================

            if (professional.User != null)
            {
                // Valida Email único
                var emailNormalized = professional.User.Email.ToLower();
                if (await _userRepository.EmailExistsAsync(emailNormalized, null, cancellationToken))
                {
                    errors.Add(_localizer.For(UserMessage.AlreadyExistsByEmail).Value);
                }
            }

            // ========================================
            // VALIDAÇÃO DAS ORGANIZAÇÕES
            // ========================================

            if (professional.Organizations != null && professional.Organizations.Any())
            {
                foreach (var orgProfessional in professional.Organizations)
                {
                    var organization = orgProfessional.Organization;

                    if (organization != null)
                    {
                        // Valida CNPJ único
                        var cnpjNormalized = organization.Cnpj.GetOnlyNumbers();
                        if (await _organizationRepository.ExistsByCnpjAsync(cnpjNormalized, cancellationToken))
                        {
                            errors.Add(_localizer.For(OrganizationMessage.AlreadyExistsByCnpj, organization.Cnpj).Value);
                        }

                        // Valida Razão Social única
                        var razaoSocialNormalized = organization.RazaoSocial.ToTitleCase();
                        if (await _organizationRepository.ExistsByRazaoSocialAsync(razaoSocialNormalized, cancellationToken))
                        {
                            errors.Add(_localizer.For(OrganizationMessage.AlreadyExistsByRazaoSocial).Value);
                        }
                    }
                }
            }

            // Se houver erros, lança exceção de conflito
            if (errors.Any())
            {
                throw new ConflictException(string.Join("; ", errors));
            }
        }
    }
}
