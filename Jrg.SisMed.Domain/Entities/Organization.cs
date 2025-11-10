using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using Jrg.SisMed.Domain.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Representa uma organização/empresa no sistema.
    /// </summary>
    public class Organization : Entity
    {
        private readonly IStringLocalizer<Messages> _localizer;

        private const int MaxNameFantasiaLength = 150;
        private const int MaxRazaoSocialLength = 150;
        private const int MaxCnpjLength = 14;

        /// <summary>
        /// Nome fantasia da organização (nome comercial).
        /// </summary>
        public string NameFantasia { get; private set; } = string.Empty;
        
        /// <summary>
        /// Razão social da organização (nome jurídico).
        /// </summary>
        public string RazaoSocial { get; private set; } = string.Empty;
        
        /// <summary>
        /// Cnpj da organização (apenas números).
        /// </summary>
        public string Cnpj { get; private set; } = string.Empty;
        
        /// <summary>
        /// Estado/status da organização no sistema.
        /// </summary>
        public OrganizationEnum.State State { get; private set; } = OrganizationEnum.State.Active;

        /// <summary>
        /// Lista de telefones da organização.
        /// </summary>
        public virtual List<OrganizationPhone> Phones { get; private set; } = new List<OrganizationPhone>();

        #region Constructors
        /// <summary>
        /// Construtor protegido para uso do Entity Framework.
        /// </summary>
        internal Organization(IStringLocalizer<Messages> localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// Cria uma nova instância de Organization com validação completa.
        /// </summary>
        /// <param name="nameFantasia">Nome fantasia (nome comercial).</param>
        /// <param name="razaoSocial">Razão social (nome jurídico).</param>
        /// <param name="cnpj">Cnpj (pode conter formatação).</param>
        /// <param name="state">Estado inicial (padrão: Active).</param>
        /// <example>
        /// <code>
        /// var org = new Organization(
        ///     nameFantasia: "Clínica Exemplo",
        ///     razaoSocial: "Clínica Exemplo Ltda",
        ///     cnpj: "12.345.678/0001-95",
        ///     state: OrganizationEnum.State.Active
        /// );
        /// </code>
        /// </example>
        public Organization(
            string nameFantasia, 
            string razaoSocial, 
            string cnpj, 
            OrganizationEnum.State state = OrganizationEnum.State.Active)
        {
            Update(nameFantasia, razaoSocial, cnpj, state);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Atualiza os dados da organização com validação completa.
        /// </summary>
        /// <param name="nameFantasia">Nome fantasia (nome comercial).</param>
        /// <param name="razaoSocial">Razão social (nome jurídico).</param>
        /// <param name="cnpj">Cnpj.</param>
        /// <param name="state">Estado da organização.</param>
        public void Update(
            string nameFantasia, 
            string razaoSocial, 
            string cnpj, 
            OrganizationEnum.State state)
        {
            // Atribui valores para validação
            NameFantasia = nameFantasia;
            RazaoSocial = razaoSocial;
            Cnpj = cnpj;
            State = state;
            
            // Normaliza e valida
            Normalize();
            Validate();
        }

        /// <summary>
        /// Adiciona um telefone à organização.
        /// </summary>
        /// <param name="phone">Relação entre organização e telefone.</param>
        /// <exception cref="ArgumentNullException">Lançado quando phone é null.</exception>
        public void AddPhone(OrganizationPhone phone)
        {
            if (phone == null)
                throw new ArgumentNullException(_localizer.For(CommonMessage.ArgumentNull, nameof(phone)));
                
            // Se for o primeiro telefone ou marcado como principal, remove flag dos outros
            if (!Phones.Any() || phone.IsPrincipal)
            {
                foreach (var p in Phones)
                    p.SetAsSecondary();
            }
            
            Phones.Add(phone);
        }

        /// <summary>
        /// Remove um telefone da organização.
        /// </summary>
        /// <param name="phone">Relação entre organização e telefone.</param>
        /// <exception cref="ArgumentNullException">Lançado quando phone é null.</exception>
        public void RemovePhone(OrganizationPhone phone)
        {
            if (phone == null)
                throw new ArgumentNullException(_localizer.For(CommonMessage.ArgumentNull, nameof(phone)));

            Phones.Remove(phone);
        }

        /// <summary>
        /// Ativa a organização no sistema.
        /// </summary>
        public void Activate()
        {
            State = OrganizationEnum.State.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Desativa a organização no sistema.
        /// </summary>
        public void Deactivate()
        {
            State = OrganizationEnum.State.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Suspende a organização no sistema.
        /// </summary>
        public void Suspend()
        {
            State = OrganizationEnum.State.Suspended;
            UpdatedAt = DateTime.UtcNow;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Valida todos os dados da organização.
        /// </summary>
        private void Validate()
        {
            var v = new ValidationCollector();

            v.When(NameFantasia.IsNullOrWhiteSpace(), _localizer.For(OrganizationMessage.TradeNameRequired));
            v.When(NameFantasia.Length > MaxNameFantasiaLength, _localizer.For(OrganizationMessage.TradeNameMaxLength, MaxNameFantasiaLength));
            
            v.When(RazaoSocial.IsNullOrWhiteSpace(), _localizer.For(OrganizationMessage.LegalNameRequired));
            v.When(RazaoSocial.Length > MaxRazaoSocialLength, _localizer.For(OrganizationMessage.LegalNameMaxLength, MaxRazaoSocialLength));

            v.When(Cnpj.IsNullOrWhiteSpace(), _localizer.For(OrganizationMessage.CnpjRequired));
            v.When(!Cnpj.IsCnpj(), _localizer.For(OrganizationMessage.CnpjInvalid));

            // Validação de enum - CORRIGIDO: Adiciona ! para inverter a lógica
            v.When(!Enum.IsDefined(typeof(OrganizationEnum.State), State), _localizer.For(OrganizationMessage.StateInvalid));

            v.ThrowIfAny();
        }

        /// <summary>
        /// Normaliza os dados da organização.
        /// </summary>
        private void Normalize()
        {
            NameFantasia = NameFantasia.RemoveDoubleSpaces().ToTitleCase();
            RazaoSocial = RazaoSocial.RemoveDoubleSpaces().ToTitleCase();
            Cnpj = Cnpj.GetOnlyNumbers();
        }
        #endregion
    }

    /// <summary>
    /// Enumerações relacionadas à entidade Organization.
    /// TODO: Mover para namespace Jrg.SisMed.Domain.Enums em refatoração futura.
    /// </summary>
    public class OrganizationEnum
    {
        /// <summary>
        /// Estado da organização no sistema.
        /// </summary>
        public enum State
        {
            /// <summary>Organização ativa e operacional.</summary>
            Active = 1,
            
            /// <summary>Organização inativa (temporariamente desabilitada).</summary>
            Inactive = 2,
            
            /// <summary>Organização suspensa (por motivos administrativos ou regulatórios).</summary>
            Suspended = 3
        }
    }
}