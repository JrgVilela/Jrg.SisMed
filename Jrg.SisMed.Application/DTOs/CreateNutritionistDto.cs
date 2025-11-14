using Jrg.SisMed.Domain.Entities;
using System;

namespace Jrg.SisMed.Application.DTOs
{
    /// <summary>
    /// DTO para criação de nutricionistas com informações específicas como CRN.
    /// Usado como contrato de entrada para criar profissionais nutricionistas.
    /// </summary>
    public record CreateNutritionistDto(
        string Name,
        string Cpf,
        string? Rg,
        DateTime? BirthDate,
        ProfessionalEnum.Gender Gender,
        string Email,
        string Password,
        string Crn
    ) : CreateProfessionalDto(Name, Cpf, Rg, BirthDate, Gender, Email, Password);
}
