using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class Psychologist : Person
    {
        private const int MaxCrpLength = 20;

        public string Crp { get; private set; } = string.Empty;

        internal Psychologist() { }

        public Psychologist(string name,
            string cpf,
            string? rg,
            DateTime? birthDate,
            PersonEnum.Gender gender,
            string email,
            string password, 
            string crp) : base(name, cpf, rg, birthDate, gender, email, password)
        {
            Crp = crp;
            Validate();
        }

        public void Update(
            string name,
            string cpf,
            string? rg,
            DateTime? birthDate,
            PersonEnum.Gender gender,
            string email,
            string password,
            string crp)
        {
            base.Update(name, cpf, rg, birthDate, gender, email, password);
            Crp = crp;
            Validate();
        }

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
