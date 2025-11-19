using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public abstract class Professional : Entity
    {
        private const int MaxNameLength = 150;
        private const int MaxCpfLength = 14;
        private const int MaxRgLength = 20;
        private const int MaxEmailLength = 100;
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 100;

        public string Name { get; private set; } = string.Empty;
        public string Cpf { get; private set; } = string.Empty;
        public string? Rg { get; private set; } = string.Empty;
        public DateTime? BirthDate { get; private set; }
        public ProfessionalEnum.Gender Gender { get; private set; } = ProfessionalEnum.Gender.None;
        public ProfessionalEnum.State State { get; private set; } = ProfessionalEnum.State.Active;

        //public string Email { get; private set; } = string.Empty;
        //public string PasswordHash { get; private set; } = string.Empty;

        public virtual List<ProfessionalAddress> Addresses { get; private set; } = new List<ProfessionalAddress>();
        public virtual List<ProfessionalPhone> Phones { get; private set; } = new List<ProfessionalPhone>();

        #region Constructors
        /// <summary>
        /// Construtor protegido para uso do Entity Framework.
        /// </summary>
        internal Professional() { }

        /// <summary>
        /// Cria uma nova instância de Professional com validação completa.
        /// </summary>
        /// <param name="name">Nome completo da pessoa.</param>
        /// <param name="cpf">CPF da pessoa (pode conter formatação).</param>
        /// <param name="rg">RG da pessoa (opcional).</param>
        /// <param name="birthDate">Data de nascimento (opcional).</param>
        /// <param name="gender">Gênero da pessoa.</param>
        /// <param name="email">E-mail da pessoa.</param>
        /// <param name="password">Senha em texto plano (será hasheada automaticamente).</param>
        public Professional(
            string name, 
            string cpf, 
            string? rg, 
            DateTime? birthDate, 
            ProfessionalEnum.Gender gender, 
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
            ProfessionalEnum.Gender gender, 
            string email, 
            string password)
        {
            // Atribui valores temporários para validação
            Name = name;
            Cpf = cpf;
            Rg = rg;
            BirthDate = birthDate;
            Gender = gender;
            
            // Valida ANTES de fazer hash da senha
            Validate();
            
            // Normaliza os dados
            Normalize();
            
            // Atualiza timestamp
            if (Id > 0) // Se já existe, atualiza UpdatedAt
                UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adiciona um endereço à pessoa.
        /// </summary>
        public void AddAddress(ProfessionalAddress address)
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
        public void AddPhone(ProfessionalPhone phone)
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
        public void RemoveAddress(ProfessionalAddress address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
                
            Addresses.Remove(address);
        }

        /// <summary>
        /// Remove um telefone da pessoa.
        /// </summary>
        public void RemovePhone(ProfessionalPhone phone)
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
            State = ProfessionalEnum.State.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Desativa a pessoa no sistema.
        /// </summary>
        public void Deactivate()
        {
            State = ProfessionalEnum.State.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }
        #endregion

        #region Private and Protected Methods
        /// <summary>
        /// Valida todos os dados da entidade.
        /// </summary>
        protected virtual void Validate()
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
            
            // Validação de Data de Nascimento
            v.When(BirthDate.HasValue && BirthDate.Value > DateTime.Now, 
                "A data de nascimento não pode ser maior que a data atual.");
            v.When(BirthDate.HasValue && BirthDate.Value < DateTime.Now.AddYears(-150), 
                "A data de nascimento é inválida.");

            // Validação de Enum - CORRIGIDO: Adiciona ! para inverter a lógica
            v.When(!Enum.IsDefined(typeof(ProfessionalEnum.Gender), Gender), 
                "O gênero informado é inválido.");
            v.When(!Enum.IsDefined(typeof(ProfessionalEnum.State), State), 
                "O status da pessoa é inválido.");
            
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
        }
        #endregion
    }

    /// <summary>
    /// Enumerações relacionadas à entidade Professional.
    /// TODO: Mover para namespace Jrg.SisMed.Domain.Enums em refatoração futura.
    /// </summary>
    public class ProfessionalEnum
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
