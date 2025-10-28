using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class Phone : EntityBase
    {
        private readonly int MinNumberLength = 8;
        private readonly int MaxNumberLength = 9;
        private readonly int DddLength = 2;
        private readonly int MinDdiLength = 2;
        private readonly int MaxDdiLength = 5;

        public string Ddi { get; private set; } = string.Empty;
        public string Ddd { get; private set; } = string.Empty;
        public string Number { get; private set; } = string.Empty;

        #region Constructors
        internal Phone() { }

        public Phone(string ddi, string ddd, string number)
        {
            Ddi = ddi;
            Ddd = ddd;
            Number = number;
        }
        #endregion

        #region Public Methods
        public void Update(string ddi, string ddd, string number)
        {
            Validate();
            Normalize();
            Ddi = ddi;
            Ddd = ddd;
            Number = number;
        }

        public override string ToString()
        {
            return Number.FormatPhoneWithDdiAndDdd(Ddi, Ddd);
        }
        #endregion

        #region Private Methods
        private void Validate()
        {
            var v = new ValidationCollector();

            v.When(Number.IsNullOrWhiteSpace(), "O número do telefone é obrigatório.");
            v.When(Ddi.IsNullOrWhiteSpace(), "O ddi do telefone é obrigatório.");
            v.When(Ddd.IsNullOrWhiteSpace(), "O ddd do telefone é obrigatório.");
            v.When(Ddi.Length < MinDdiLength || Ddi.Length > MaxDdiLength, $"O código do país (DDI) deve conter entre {MinDdiLength} e {MaxDdiLength} dígitos.");
            v.When(Ddd.Length != DddLength, $"O código de área (DDD) deve conter {DddLength} dígitos.");
            v.When(Number.Length < MinNumberLength || Number.Length > MaxNumberLength, $"O número de telefone deve conter entre {MinNumberLength} e {MaxNumberLength} dígitos.");

            v.ThrowIfAny();
        }

        private void Normalize()
        {
            Ddi = Ddi.Trim();
            Ddd = Ddd.Trim();
            Number = Number.Trim().FormatPhone();
        }
        #endregion


    }
}
