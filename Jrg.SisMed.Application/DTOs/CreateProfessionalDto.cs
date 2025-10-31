using Jrg.SisMed.Domain.Entities;
using System;

namespace Jrg.SisMed.Application.DTOs
{
    /// <summary>
    /// DTO base abstrato para criação de profissionais.
    /// Usado como contrato de entrada para casos de uso da Application Layer.
    /// </summary>
    public abstract record CreateProfessionalDto(
        string Name,
        string Cpf,
        string? Rg,
        DateTime? BirthDate,
        PersonEnum.Gender Gender,
        string Email,
        string Password
    );
}
