using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class Person : Entity
    {
        private const int MaxNameLength = 150;
        private const int MaxCpfLength = 11;
        private const int MaxRgLength = 20;
        private const int MaxEmailLength = 100;
        private const int MinPasswordHashLength = 6;
        private const int MaxPasswordHashLength = 30;

        public string Name { get; private set; } = string.Empty;
        public string Cpf { get; private set; } = string.Empty;
        public string? Rg { get; private set; } = string.Empty;
        public DateTime? BirthDate { get; private set; }
        public PersonEnum.Gender Gender { get; private set; } = PersonEnum.Gender.None;
        public PersonEnum.State State { get; set; }

        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;

        public virtual List<PersonAddress> Addresses { get; private set; } = new List<PersonAddress>();
        public virtual List<PersonPhone> Phones { get; private set; } = new List<PersonPhone>();

        #region Constructors
        internal Person() { }

        //Preciso criar um construtor que preencha todos os campos
        public Person(string name, string cpf, string? rg, DateTime? birthDate, PersonEnum.Gender gender, string email, string passwordHash)
        {
            Update(name, cpf, rg, birthDate, gender, email, passwordHash);
        }
        #endregion

        #region Public Methods
        public void Update(string name, string cpf, string? rg, DateTime? birthDate, PersonEnum.Gender gender, string email, string passwordHash)
        {
            Validate();
            Name = name;
            Cpf = cpf.FormatCpf();
            Rg = rg;
            BirthDate = birthDate;
            Gender = gender;
            Email = email;
            PasswordHash = SecurityHelper.HashPasswordPbkdf2(passwordHash);
        }

        public void AddAddress(PersonAddress address)
        {
            Addresses.Add(address);
        }

        public void AddPhone(PersonPhone phone)
        {
            Phones.Add(phone);
        }

        public void RemoveAddress(PersonAddress address)
        {
            Addresses.Remove(address);
        }

        public void RemovePhone(PersonPhone phone)
        {
            Phones.Remove(phone);
        }

        public void Activate()
        {
            State = PersonEnum.State.Active;
        }

        public void Deactivate()
        {
            State = PersonEnum.State.Inactive;
        }
        #endregion

        #region Private Methods
        private void Validate()
        {
            Normalize();
            var v = new ValidationCollector();

            v.When(Name.IsNullOrWhiteSpace(), "O nome é obrigatório.");
            v.When(Name.Length > MaxNameLength, $"O nome deve conter no máximo {MaxNameLength} caracteres.");
            
            v.When(Cpf.IsNullOrWhiteSpace(), "O CPF é obrigatório.");
            v.When(Cpf.Length > MaxCpfLength, $"O CPF deve conter no máximo {MaxCpfLength} caracteres.");
            v.When(!Cpf.IsCpf(), "O CPF informado é inválido.");
            
            v.When(!Rg!.IsNullOrWhiteSpace() && Rg!.Length > MaxRgLength, $"O RG deve conter no máximo {MaxRgLength} caracteres.");
            
            v.When(Email.IsNullOrWhiteSpace(), "O e-mail é obrigatório.");
            v.When(Email.Length > MaxEmailLength, $"O e-mail deve conter no máximo {MaxEmailLength} caracteres.");
            v.When(!Email.IsEmail(), "O e-mail informado é inválido.");

            v.When(BirthDate > DateTime.Now, "A data de nascimento não pode ser maior que a data atual.");

            v.When(Enum.IsDefined(typeof(PersonEnum.Gender), Gender), "O gênero informado é inválido.");

            v.When(Enum.IsDefined(typeof(PersonEnum.State), State), "O status da pessoa é inválido.");

            v.When(PasswordHash.IsNullOrWhiteSpace(), "A senha é obrigatória.");
            v.When(PasswordHash.Length < MinPasswordHashLength || PasswordHash.Length > MaxPasswordHashLength, $"A senha deve conter entre {MinPasswordHashLength} e {MaxPasswordHashLength} caracteres.");
            
            v.ThrowIfAny();
        }

        private void Normalize()
        {
            Name = Name.RemoveDoubleSpaces().ToTitleCase();
            Cpf = Cpf.GetOnlyNumbers();
            Rg = Rg?.GetOnlyNumbers();
            Email = Email.RemoveAllSpaces().ToLower();
        }
        #endregion
    }

    public class PersonEnum
    {
        public enum State
        {
            Active = 1,
            Inactive = 2
        }

        public enum Gender
        {
            None = 0,
            Male = 1,
            Female = 2,
            Other = 3
        }
    }
}
