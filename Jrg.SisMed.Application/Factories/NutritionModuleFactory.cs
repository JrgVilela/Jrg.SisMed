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
    /// Factory responsável por criar instâncias de profissionais de nutrição.
    /// Esta implementação na Application Layer usa DTOs para receber dados estruturados.
    /// </summary>
    [ProfessionalType(ProfessionalType.Nutritionist)]
    public class NutritionModuleFactory : IProfessionalModuleFactory
    {
        /// <summary>
        /// Cria um novo profissional nutricionista com validações específicas usando DTO.
        /// </summary>
        /// <param name="dto">Dados para criação do nutricionista.</param>
        /// <returns>Instância de Nutritionist.</returns>
        /// <exception cref="ArgumentException">Quando o DTO não é do tipo CreateNutritionistDto.</exception>
        /// <exception cref="DomainValidationException">Quando o CRN é inválido.</exception>
        public Professional CreateProfessionalFromDto(CreateProfessionalDto dto)
        {
            if (dto is not CreateNutritionistDto nutritionistDto)
                throw new ArgumentException("DTO inválido para NutritionModuleFactory. Esperado CreateNutritionistDto.", nameof(dto));

            // Validar CRN específico
            if (!ValidateCrn(nutritionistDto.Crn))
                throw new DomainValidationException(new[] { "CRN inválido. Deve ter pelo menos 5 caracteres." });

            return CreateProfessional(
                nutritionistDto.Name,
                nutritionistDto.Cpf,
                nutritionistDto.Rg,
                nutritionistDto.BirthDate,
                nutritionistDto.Gender,
                nutritionistDto.Email,
                nutritionistDto.Password,
                nutritionistDto.Crn
            );
        }

        /// <summary>
        /// Implementação da interface Domain. Cria um nutricionista com parâmetros primitivos.
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
            // Validar CRN específico
            if (!ValidateCrn(professionalRegistration))
                throw new DomainValidationException(new[] { "CRN inválido. Deve ter pelo menos 5 caracteres." });

            return new Nutritionist(
                name,
                cpf,
                rg,
                birthDate,
                gender,
                email,
                password,
                NormalizeCrn(professionalRegistration)
            );
        }

        /// <summary>
        /// Valida o formato do CRN.
        /// </summary>
        /// <param name="crn">Número do CRN a ser validado.</param>
        /// <returns>True se o CRN é válido, false caso contrário.</returns>
        private static bool ValidateCrn(string crn)
        {
            return !string.IsNullOrWhiteSpace(crn) && crn.Trim().Length >= 5;
        }

        /// <summary>
        /// Normaliza o CRN removendo espaços e convertendo para maiúsculas.
        /// </summary>
        /// <param name="crn">CRN a ser normalizado.</param>
        /// <returns>CRN normalizado.</returns>
        private static string NormalizeCrn(string crn)
        {
            return crn.Trim().ToUpperInvariant();
        }

        //public Professional IProfessionalModuleFactory.CreateProfessional(string name, string cpf, string? rg, DateTime? birthDate, PersonEnum.Gender gender, string email, string password, string professionalRegistration)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
