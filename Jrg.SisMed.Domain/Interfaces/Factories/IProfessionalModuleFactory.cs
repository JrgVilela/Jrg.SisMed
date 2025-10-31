using Jrg.SisMed.Domain.Entities;

namespace Jrg.SisMed.Domain.Interfaces.Factories
{
    /// <summary>
    /// Interface base para factories de módulos profissionais.
    /// Cada factory é responsável por criar instâncias específicas de profissionais.
    /// Esta interface pertence ao Domain, mas as implementações concretas na Application Layer
    /// podem receber DTOs e convertê-los internamente.
    /// </summary>
    public interface IProfessionalModuleFactory
    {
        /// <summary>
        /// Cria uma nova instância de profissional.
        /// As implementações concretas na Application Layer definirão os parâmetros específicos
        /// através de métodos adicionais ou sobrecarga.
        /// </summary>
        /// <returns>Instância de Person (Psychologist ou Nutritionist).</returns>
        Person CreateProfessional(
            string name,
            string cpf,
            string? rg,
            System.DateTime? birthDate,
            PersonEnum.Gender gender,
            string email,
            string password,
            string professionalRegistration
        );
    }
}
