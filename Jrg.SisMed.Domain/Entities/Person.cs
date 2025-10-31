using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public abstract class Person : Entity
    {
        private const int MaxNameLength = 150;
        private const int MaxCpfLength = 11;
        private const int MaxRgLength = 20;
        private const int MaxEmailLength = 100;
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 100;

        public string Name { get; private set; } = string.Empty;
        public string Cpf { get; private set; } = string.Empty;
        public string? Rg { get; private set; } = string.Empty;
        public DateTime? BirthDate { get; private set; }
        public PersonEnum.Gender Gender { get; private set; } = PersonEnum.Gender.None;
        public PersonEnum.State State { get; private set; } = PersonEnum.State.Active;

        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;

        public virtual List<PersonAddress> Addresses { get; private set; } = new List<PersonAddress>();
        public virtual List<PersonPhone> Phones { get; private set; } = new List<PersonPhone>();

        #region Constructors
        /// <summary>
        /// Construtor protegido para uso do Entity Framework.
        /// </summary>
        internal Person() { }

        /// <summary>
        /// Cria uma nova instância de Person com validação completa.
        /// </summary>
        /// <param name="name">Nome completo da pessoa.</param>
        /// <param name="cpf">CPF da pessoa (pode conter formatação).</param>
        /// <param name="rg">RG da pessoa (opcional).</param>
        /// <param name="birthDate">Data de nascimento (opcional).</param>
        /// <param name="gender">Gênero da pessoa.</param>
        /// <param name="email">E-mail da pessoa.</param>
        /// <param name="password">Senha em texto plano (será hasheada automaticamente).</param>
        public Person(
            string name, 
            string cpf, 
            string? rg, 
            DateTime? birthDate, 
            PersonEnum.Gender gender, 
            string email, 
            string password)
        {
            Update(name, cpf, rg, birthDate, gender, email, password);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Atualiza os dados da pessoa com validação completa.
        /// </summary>
        /// <param name="name">Nome completo da pessoa.</param>
        /// <param name="cpf">CPF da pessoa.</param>
        /// <param name="rg">RG da pessoa (opcional).</param>
        /// <param name="birthDate">Data de nascimento (opcional).</param>
        /// <param name="gender">Gênero da pessoa.</param>
        /// <param name="email">E-mail da pessoa.</param>
        /// <param name="password">Senha em texto plano (será hasheada automaticamente).</param>
        public void Update(
            string name, 
            string cpf, 
            string? rg, 
            DateTime? birthDate, 
            PersonEnum.Gender gender, 
            string email, 
            string password)
        {
            // Atribui valores temporários para validação
            Name = name;
            Cpf = cpf;
            Rg = rg;
            BirthDate = birthDate;
            Gender = gender;
            Email = email;
            
            // Valida ANTES de fazer hash da senha
            ValidatePassword(password);
            Validate();
            
            // Normaliza os dados
            Normalize();
            
            // Hash da senha APÓS validação bem-sucedida
            PasswordHash = SecurityHelper.HashPasswordPbkdf2(password);
            
            // Atualiza timestamp
            if (Id > 0) // Se já existe, atualiza UpdatedAt
                UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adiciona um endereço à pessoa.
        /// </summary>
        public void AddAddress(PersonAddress address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
                
            // Se for o primeiro endereço ou marcado como principal, remove flag dos outros
            if (!Addresses.Any() || address.IsPrincipal)
            {
                foreach (var addr in Addresses)
                    addr.SetAsSecondary();
            }
            
            Addresses.Add(address);
        }

        /// <summary>
        /// Adiciona um telefone à pessoa.
        /// </summary>
        public void AddPhone(PersonPhone phone)
        {
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));
                
            // Se for o primeiro telefone ou marcado como principal, remove flag dos outros
            if (!Phones.Any() || phone.IsPrincipal)
            {
                foreach (var p in Phones)
                    p.SetAsSecondary();
            }
            
            Phones.Add(phone);
        }

        /// <summary>
        /// Remove um endereço da pessoa.
        /// </summary>
        public void RemoveAddress(PersonAddress address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
                
            Addresses.Remove(address);
        }

        /// <summary>
        /// Remove um telefone da pessoa.
        /// </summary>
        public void RemovePhone(PersonPhone phone)
        {
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));
                
            Phones.Remove(phone);
        }

        /// <summary>
        /// Ativa a pessoa no sistema.
        /// </summary>
        public void Activate()
        {
            State = PersonEnum.State.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Desativa a pessoa no sistema.
        /// </summary>
        public void Deactivate()
        {
            State = PersonEnum.State.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Verifica se a senha fornecida corresponde ao hash armazenado.
        /// </summary>
        public bool VerifyPassword(string password)
        {
            if (password.IsNullOrWhiteSpace())
                return false;
                
            return SecurityHelper.VerifyPasswordPbkdf2(password, PasswordHash);
        }

        /// <summary>
        /// Altera a senha da pessoa.
        /// </summary>
        public void ChangePassword(string currentPassword, string newPassword)
        {
            if (!VerifyPassword(currentPassword))
                throw new DomainValidationException(new[] { "A senha atual está incorreta." });
                
            ValidatePassword(newPassword);
            PasswordHash = SecurityHelper.HashPasswordPbkdf2(newPassword);
            UpdatedAt = DateTime.UtcNow;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Valida todos os dados da entidade.
        /// </summary>
        private void Validate()
        {
            var v = new ValidationCollector();

            // Validação de Nome
            v.When(Name.IsNullOrWhiteSpace(), "O nome é obrigatório.");
            v.When(Name.Length > MaxNameLength, $"O nome deve conter no máximo {MaxNameLength} caracteres.");
            
            // Validação de CPF
            v.When(Cpf.IsNullOrWhiteSpace(), "O CPF é obrigatório.");
            v.When(!Cpf.IsCpf(), "O CPF informado é inválido.");
            
            // Validação de RG (opcional)
            v.When(!Rg.IsNullOrWhiteSpace() && Rg!.Length > MaxRgLength, 
                $"O RG deve conter no máximo {MaxRgLength} caracteres.");
            
            // Validação de E-mail
            v.When(Email.IsNullOrWhiteSpace(), "O e-mail é obrigatório.");
            v.When(Email.Length > MaxEmailLength, $"O e-mail deve conter no máximo {MaxEmailLength} caracteres.");
            v.When(!Email.IsEmail(), "O e-mail informado é inválido.");

            // Validação de Data de Nascimento
            v.When(BirthDate.HasValue && BirthDate.Value > DateTime.Now, 
                "A data de nascimento não pode ser maior que a data atual.");
            v.When(BirthDate.HasValue && BirthDate.Value < DateTime.Now.AddYears(-150), 
                "A data de nascimento é inválida.");

            // Validação de Enum - CORRIGIDO: Adiciona ! para inverter a lógica
            v.When(!Enum.IsDefined(typeof(PersonEnum.Gender), Gender), 
                "O gênero informado é inválido.");
            v.When(!Enum.IsDefined(typeof(PersonEnum.State), State), 
                "O status da pessoa é inválido.");
            
            v.ThrowIfAny();
        }

        /// <summary>
        /// Valida a senha antes de fazer hash.
        /// </summary>
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

        /// <summary>
        /// Normaliza os dados antes de armazenar.
        /// </summary>
        private void Normalize()
        {
            Name = Name.RemoveDoubleSpaces().ToTitleCase();
            Cpf = Cpf.GetOnlyNumbers();
            Rg = Rg?.GetOnlyNumbers();
            Email = Email.RemoveAllSpaces().ToLower();
        }
        #endregion
    }

    /// <summary>
    /// Enumerações relacionadas à entidade Person.
    /// TODO: Mover para namespace Jrg.SisMed.Domain.Enums em refatoração futura.
    /// </summary>
    public class PersonEnum
    {
        /// <summary>
        /// Estado da pessoa no sistema.
        /// </summary>
        public enum State
        {
            /// <summary>Pessoa ativa no sistema.</summary>
            Active = 1,
            
            /// <summary>Pessoa inativa no sistema.</summary>
            Inactive = 2
        }

        /// <summary>
        /// Gênero da pessoa.
        /// </summary>
        public enum Gender
        {
            /// <summary>Não especificado.</summary>
            None = 0,
            
            /// <summary>Masculino.</summary>
            Male = 1,
            
            /// <summary>Feminino.</summary>
            Female = 2,
            
            /// <summary>Outro.</summary>
            Other = 3
        }
    }
}
