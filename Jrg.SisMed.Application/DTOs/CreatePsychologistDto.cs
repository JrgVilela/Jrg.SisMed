using Jrg.SisMed.Domain.Entities;
using System;

namespace Jrg.SisMed.Application.DTOs
{
    /// <summary>
    /// DTO para criação de psicólogos com informações específicas como CRP.
    /// Usado como contrato de entrada para criar profissionais psicólogos.
    /// </summary>
    public record CreatePsychologistDto(
        string Name,
        string Cpf,
        string? Rg,
        DateTime? BirthDate,
        PersonEnum.Gender Gender,
        string Email,
        string Password,
        string Crp
    ) : CreateProfessionalDto(Name, Cpf, Rg, BirthDate, Gender, Email, Password);
}
