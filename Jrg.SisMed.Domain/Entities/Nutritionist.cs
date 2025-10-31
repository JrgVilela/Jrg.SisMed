using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class Nutritionist : Person
    {
        private const int MaxCrnLength = 20;
        public string Crn { get; private set; } = string.Empty;

        internal Nutritionist() { }

        public Nutritionist(string name,
            string cpf,
            string? rg,
            DateTime? birthDate,
            PersonEnum.Gender gender,
            string email,
            string password, string crn) : base(name, cpf, rg, birthDate, gender, email, password)
        {
            Crn = crn;
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
            string crn)
        {
            base.Update(name, cpf, rg, birthDate, gender, email, password);
            Crn = crn;
            Validate();
        }

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
