using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um nutricionista no sistema.
    /// Contém informações específicas como CRN (Conselho Regional de Nutricionistas).
    /// </summary>
    public class Nutritionist : Person
    {
        private const int MaxCrnLength = 20;
        
        /// <summary>
        /// Número do CRN (Conselho Regional de Nutricionistas).
        /// </summary>
        public string Crn { get; private set; } = string.Empty;

        internal Nutritionist() { }

        /// <summary>
        /// Inicializa uma nova instância de Nutritionist.
        /// </summary>
        /// <param name="name">Nome completo do nutricionista.</param>
        /// <param name="cpf">CPF do nutricionista.</param>
        /// <param name="rg">RG do nutricionista (opcional).</param>
        /// <param name="birthDate">Data de nascimento (opcional).</param>
        /// <param name="gender">Gênero do nutricionista.</param>
        /// <param name="email">Email do nutricionista.</param>
        /// <param name="password">Senha para acesso ao sistema.</param>
        /// <param name="crn">Número do CRN.</param>
        public Nutritionist(string name,
            string cpf,
            string? rg,
            DateTime? birthDate,
            PersonEnum.Gender gender,
            string email,
            string password, 
            string crn) : base(name, cpf, rg, birthDate, gender, email, password)
        {
            Crn = crn;
            Normalize();
            Validate();
        }

        /// <summary>
        /// Atualiza os dados do nutricionista.
        /// </summary>
        /// <param name="name">Nome completo.</param>
        /// <param name="cpf">CPF.</param>
        /// <param name="rg">RG (opcional).</param>
        /// <param name="birthDate">Data de nascimento (opcional).</param>
        /// <param name="gender">Gênero.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Senha.</param>
        /// <param name="crn">Número do CRN.</param>
        public void Update(
            string name,
            string cpf,
            string? rg,
            DateTime? birthDate,
            PersonEnum.Gender gender,
            string email,
            string password,
            string crn)
        {
            base.Update(name, cpf, rg, birthDate, gender, email, password);
            Crn = crn;
            Normalize();
            Validate();
        }

        /// <summary>
        /// Normaliza os dados específicos do nutricionista (CRN).
        /// </summary>
        private void Normalize()
        {
            Crn = Crn.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Valida os dados específicos do nutricionista.
        /// </summary>
        protected override void Validate()
        {
            base.Validate();
            var v = new ValidationCollector();
            
            v.When(Crn.IsNullOrWhiteSpace(), "O CRN é obrigatório.");
            v.When(Crn.Length > MaxCrnLength, $"O CRN deve conter no máximo {MaxCrnLength} caracteres.");
            
            v.ThrowIfAny();
        }
    }
}
