using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um psicólogo no sistema.
    /// Contém informações específicas como CRP (Conselho Regional de Psicologia).
    /// </summary>
    public class Psychologist : Professional
    {
        private const int MaxCrpLength = 20;

        /// <summary>
        /// Número do CRP (Conselho Regional de Psicologia).
        /// </summary>
        public string Crp { get; private set; } = string.Empty;

        internal Psychologist() { }

        /// <summary>
        /// Inicializa uma nova instância de Psychologist.
        /// </summary>
        /// <param name="name">Nome completo do psicólogo.</param>
        /// <param name="cpf">CPF do psicólogo.</param>
        /// <param name="rg">RG do psicólogo (opcional).</param>
        /// <param name="birthDate">Data de nascimento (opcional).</param>
        /// <param name="gender">Gênero do psicólogo.</param>
        /// <param name="email">Email do psicólogo.</param>
        /// <param name="password">Senha para acesso ao sistema.</param>
        /// <param name="crp">Número do CRP.</param>
        public Psychologist(string name,
            string cpf,
            string? rg,
            DateTime? birthDate,
            ProfessionalEnum.Gender gender,
            string email,
            string password, 
            string crp) : base(name, cpf, rg, birthDate, gender, email, password)
        {
            Crp = crp;
            Normalize();
            Validate();
        }

        /// <summary>
        /// Atualiza os dados do psicólogo.
        /// </summary>
        /// <param name="name">Nome completo.</param>
        /// <param name="cpf">CPF.</param>
        /// <param name="rg">RG (opcional).</param>
        /// <param name="birthDate">Data de nascimento (opcional).</param>
        /// <param name="gender">Gênero.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Senha.</param>
        /// <param name="crp">Número do CRP.</param>
        public void Update(
            string name,
            string cpf,
            string? rg,
            DateTime? birthDate,
            ProfessionalEnum.Gender gender,
            string email,
            string password,
            string crp)
        {
            base.Update(name, cpf, rg, birthDate, gender, email, password);
            Crp = crp;
            Normalize();
            Validate();
        }

        /// <summary>
        /// Normaliza os dados específicos do psicólogo (CRP).
        /// </summary>
        private void Normalize()
        {
            Crp = Crp.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Valida os dados específicos do psicólogo.
        /// </summary>
        protected override void Validate()
        {
            base.Validate();
            var v = new ValidationCollector();

            v.When(Crp.IsNullOrWhiteSpace(), "O CRP é obrigatório.");
            v.When(Crp.Length > MaxCrpLength, $"O CRP deve conter no máximo {MaxCrpLength} caracteres.");

            v.ThrowIfAny();
        }
    }
}
