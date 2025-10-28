using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Representa o relacionamento entre Organization e Phone.
    /// </summary>
    public class OrganizationPhone : EntityBase
    {
        /// <summary>
        /// ID da organização.
        /// </summary>
        [ForeignKey(nameof(Organization))]
        public int OrganizationId { get; set; }
        
        /// <summary>
        /// Navegação para a organização.
        /// </summary>
        public virtual Organization Organization { get; set; } = null!;

        /// <summary>
        /// ID do telefone.
        /// </summary>
        [ForeignKey(nameof(Phone))]
        public int PhoneId { get; set; }
        
        /// <summary>
        /// Navegação para o telefone.
        /// </summary>
        public virtual Phone Phone { get; set; } = null!;

        /// <summary>
        /// Indica se este é o telefone principal da organização.
        /// </summary>
        public bool IsPrincipal { get; private set; }

        #region Constructors
        /// <summary>
        /// Construtor protegido para uso do Entity Framework.
        /// </summary>
        internal OrganizationPhone() { }

        /// <summary>
        /// Cria uma nova relação entre organização e telefone.
        /// </summary>
        /// <param name="organization">Organização dona do telefone.</param>
        /// <param name="phone">Telefone a ser associado.</param>
        /// <param name="isPrincipal">Se é o telefone principal.</param>
        /// <exception cref="ArgumentNullException">Lançado quando organization ou phone é null.</exception>
        public OrganizationPhone(Organization organization, Phone phone, bool isPrincipal = false)
        {
            Organization = organization ?? throw new ArgumentNullException(nameof(organization));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            OrganizationId = organization.Id;
            PhoneId = phone.Id;
            IsPrincipal = isPrincipal;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Define este telefone como principal.
        /// Remove a flag de principal dos outros telefones automaticamente.
        /// </summary>
        public void SetAsPrincipal()
        {
            IsPrincipal = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Define este telefone como secundário.
        /// </summary>
        public void SetAsSecondary()
        {
            IsPrincipal = false;
            UpdatedAt = DateTime.UtcNow;
        }
        #endregion
    }
}