using Jrg.SisMed.Domain.Enumerators;
using System;

namespace Jrg.SisMed.Domain.Attributes
{
    /// <summary>
    /// Atributo para identificar o tipo de profissional que uma factory cria.
    /// Usado pelo Provider para resolver factories dinamicamente.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ProfessionalTypeAttribute : Attribute
    {
        /// <summary>
        /// Tipo de profissional que a factory cria.
        /// </summary>
        public ProfessionalType Type { get; }

        /// <summary>
        /// Inicializa uma nova instância do atributo.
        /// </summary>
        /// <param name="type">Tipo de profissional.</param>
        public ProfessionalTypeAttribute(ProfessionalType type)
        {
            Type = type;
        }
    }
}
