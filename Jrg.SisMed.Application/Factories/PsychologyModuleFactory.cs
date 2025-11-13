using Jrg.SisMed.Application.DTOs;
using Jrg.SisMed.Domain.Attributes;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Enumerators;
using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Interfaces.Factories.Professional;
using System;

namespace Jrg.SisMed.Application.Factories
{
    /// <summary>
    /// Factory responsável por criar instâncias de profissionais de psicologia.
    /// Esta implementação na Application Layer usa DTOs para receber dados estruturados.
    /// </summary>
    [ProfessionalType(ProfessionalType.Psychologist)]
    public class PsychologyModuleFactory : IProfessionalModuleFactory
    {
        /// <summary>
        /// Cria um novo profissional psicólogo com validações específicas usando DTO.
        /// </summary>
        /// <param name="dto">Dados para criação do psicólogo.</param>
        /// <returns>Instância de Psychologist.</returns>
        /// <exception cref="ArgumentException">Quando o DTO não é do tipo CreatePsychologistDto.</exception>
        /// <exception cref="DomainValidationException">Quando o CRP é inválido.</exception>
        public Professional CreateProfessionalFromDto(CreateProfessionalDto dto)
        {
            if (dto is not CreatePsychologistDto psychologistDto)
                throw new ArgumentException("DTO inválido para PsychologyModuleFactory. Esperado CreatePsychologistDto.", nameof(dto));

            // Validar CRP específico
            if (!ValidateCrp(psychologistDto.Crp))
                throw new DomainValidationException(new[] { "CRP inválido. Deve ter pelo menos 5 caracteres." });

            return CreateProfessional(
                psychologistDto.Name,
                psychologistDto.Cpf,
                psychologistDto.Rg,
                psychologistDto.BirthDate,
                psychologistDto.Gender,
                psychologistDto.Email,
                psychologistDto.Password,
                psychologistDto.Crp
            );
        }

        /// <summary>
        /// Implementação da interface Domain. Cria um psicólogo com parâmetros primitivos.
        /// </summary>
        public Professional CreateProfessional(
            string name,
            string cpf,
            string? rg,
            DateTime? birthDate,
            PersonEnum.Gender gender,
            string email,
            string password,
            string professionalRegistration)
        {
            // Validar CRP específico
            if (!ValidateCrp(professionalRegistration))
                throw new DomainValidationException(new[] { "CRP inválido. Deve ter pelo menos 5 caracteres." });

            return new Psychologist(
                name,
                cpf,
                rg,
                birthDate,
                gender,
                email,
                password,
                NormalizeCrp(professionalRegistration)
            );
        }

        /// <summary>
        /// Valida o formato do CRP.
        /// </summary>
        /// <param name="crp">Número do CRP a ser validado.</param>
        /// <returns>True se o CRP é válido, false caso contrário.</returns>
        private static bool ValidateCrp(string crp)
        {
            return !string.IsNullOrWhiteSpace(crp) && crp.Trim().Length >= 5;
        }

        /// <summary>
        /// Normaliza o CRP removendo espaços e convertendo para maiúsculas.
        /// </summary>
        /// <param name="crp">CRP a ser normalizado.</param>
        /// <returns>CRP normalizado.</returns>
        private static string NormalizeCrp(string crp)
        {
            return crp.Trim().ToUpperInvariant();
        }
    }
}
