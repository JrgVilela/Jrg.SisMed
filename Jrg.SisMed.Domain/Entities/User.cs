using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Jrg.SisMed.Domain.Entities.ProfessionalEnum;

namespace Jrg.SisMed.Domain.Entities
{
    public class User : Entity
    {
        private const int MaxNameLength = 100;
        private const int MaxEmailLength = 100;
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 25;


        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public UserEnum.State State { get; private set; }

        #region Constructors

        internal User() { }

        public User(string name, string email, string password, UserEnum.State state)
        {
            SetProperties(name, email, password, state);

            if(Id == 0) // Se for novo, seta CreatedAt
                CreatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Public Methods
        public void Update(string name, string email, string password, UserEnum.State state)
        {
            SetProperties(name, email, password, state);

            if (Id > 0) // Se já existe, atualiza UpdatedAt
                UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            State = UserEnum.State.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            State = UserEnum.State.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool VerifyPassword(string password)
        {
            if (password.IsNullOrWhiteSpace())
                return false;

            return SecurityHelper.VerifyPasswordPbkdf2(password, Password);
        }
        #endregion

        #region Private Methods
        private void SetProperties(string name, string email, string password, UserEnum.State state)
        {
            Name = name;
            Email = email;
            State = state;

            ValidatePassword(password);
            Validate();

            Normalize();

            Password = SecurityHelper.HashPasswordPbkdf2(password);
        }


        private void Validate()
        {
            var v = new ValidationCollector();

            // Validação de Nome
            v.When(Name.IsNullOrWhiteSpace(), "O nome é obrigatório.");
            v.When(Name.Length > MaxNameLength, $"O nome deve conter no máximo {MaxNameLength} caracteres.");

            // Validação de E-mail
            v.When(Email.IsNullOrWhiteSpace(), "O e-mail é obrigatório.");
            v.When(Email.Length > MaxEmailLength, $"O e-mail deve conter no máximo {MaxEmailLength} caracteres.");
            v.When(!Email.IsEmail(), "O e-mail informado é inválido.");

            v.When(!Enum.IsDefined(typeof(ProfessionalEnum.State), State),
                "O status da pessoa é inválido.");

            v.ThrowIfAny();
        }

        private void ValidatePassword(string password)
        {
            var v = new ValidationCollector();

            v.When(password.IsNullOrWhiteSpace(), "A senha é obrigatória.");
            v.When(password.Length < MinPasswordLength || password.Length > MaxPasswordLength,
                $"A senha deve conter entre {MinPasswordLength} e {MaxPasswordLength} caracteres.");

            // Validação de força de senha
            if (!password.IsNullOrWhiteSpace())
            {
                v.When(!SecurityHelper.IsPasswordStrong(password, minLength: MinPasswordLength),
                    "A senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais.");
            }

            v.ThrowIfAny();
        }

        private void Normalize()
        {
            Name = Name.Trim().ToTitleCase();
            Email = Email.Trim().ToLower();
        }

        #endregion
    }

    public class UserEnum
    {
        public enum State
        {
            Active = 1,
            Inactive = 2,
            Blocked = 3
        }
    }
}
