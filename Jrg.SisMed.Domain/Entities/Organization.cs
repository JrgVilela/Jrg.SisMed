using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class Organization : Entity
    {
        private readonly int MaxNameFantasiaLength = 150;
        private readonly int MaxRazaoSocialLength = 150;

        public string NameFantasia { get; private set; } = string.Empty;
        public string RazaoSocial { get; private set; } = string.Empty;
        public string CNPJ { get; private set; } = string.Empty;
        public OrganizationEnum.State State { get; private set; } = OrganizationEnum.State.Active;

        public virtual List<OrganizationPhone> Phones { get; private set; } = new List<OrganizationPhone>();

        #region Constructors
        internal Organization() { }
        public Organization(string nameFantasia, string razaoSocial, string cnpj, OrganizationEnum.State status)
        {
            Validate();

            NameFantasia = nameFantasia;
            RazaoSocial = razaoSocial;
            CNPJ = cnpj;
            State = status;
        }
        #endregion

        #region Public Methods
        public void Update(string nameFantasia, string razaoSocial, string cnpj, OrganizationEnum.State status)
        {
            Validate();
            NameFantasia = nameFantasia;
            RazaoSocial = razaoSocial;
            CNPJ = cnpj;
            State = status;
        }

        public void AddPhone(OrganizationPhone phone)
        {
            Phones.Add(phone);
        }

        public void RemovePhone(OrganizationPhone phone)
        {
            Phones.Remove(phone);
        }
        #endregion

        #region Private Methods
        public void Validate()
        {
            var v = new ValidationCollector();

            Normalize();
            v.When(string.IsNullOrWhiteSpace(NameFantasia), "O nome fantasia é obrigatório.");
            v.When(NameFantasia.Length > MaxNameFantasiaLength, $"O nome fantasia deve conter no máximo {MaxNameFantasiaLength} caracteres.");
            v.When(string.IsNullOrWhiteSpace(RazaoSocial), "A razão social é obrigatória.");
            v.When(RazaoSocial.Length > MaxRazaoSocialLength, $"A razão social deve conter no máximo {MaxRazaoSocialLength} caracteres.");
            v.When(string.IsNullOrWhiteSpace(CNPJ), "O CNPJ é obrigatório.");
            v.When(!CNPJ.IsCnpj(), "O CNPJ informado é inválido.");
            v.When(!Enum.IsDefined(typeof(OrganizationEnum.State), State), "O status da organização é inválido.");

            v.ThrowIfAny();
        }

        private void Normalize()
        {
            RazaoSocial.RemoveAllSpaces().ToTitleCase();
            NameFantasia.RemoveAllSpaces().ToTitleCase();
            CNPJ.FormatCnpj();
        }
        #endregion
    }

    public class OrganizationEnum
    {
        public enum State
        {
            Active = 1,
            Inactive = 2
        }
    }
}