
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Enumerators;
using Jrg.SisMed.Domain.Interfaces.Services.ProfessionalServices;

namespace Jrg.SisMed.Domain.Interfaces.Factories.ProfessionalFactories
{
    /// <summary>
    /// Interface base para factories de módulos profissionais.
    /// Cada factory é responsável por criar instâncias específicas de profissionais.
    /// Esta interface pertence ao Domain, mas as implementações concretas na Application Layer
    /// podem receber DTOs e convertê-los internamente.
    /// </summary>
    public interface IProfessionalModuleFactory
    {
        ProfessionalType Type { get; }
        object CreateRegister();
    }
}
