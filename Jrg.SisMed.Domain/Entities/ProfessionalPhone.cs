using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Representa o relacionamento entre professional e Phone.
    /// </summary>
    public class ProfessionalPhone : EntityBase
    {
        /// <summary>
        /// ID da professional.
        /// </summary>
        [ForeignKey(nameof(Professional))]
        public int ProfessionalId { get; set; }

        /// <summary>
        /// Navegação para a professional.
        /// </summary>
        public virtual Professional Professional { get; set; } = null!;

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
        /// Indica se este é o telefone principal da professional.
        /// </summary>
        public bool IsPrincipal { get; private set; }

        #region Constructors
        /// <summary>
        /// Construtor protegido para uso do Entity Framework.
        /// </summary>
        internal ProfessionalPhone() { }

        /// <summary>
        /// Cria uma nova relação entre professional e telefone.
        /// </summary>
        /// <param name="professional">professional dona do telefone.</param>
        /// <param name="phone">Telefone a ser associado.</param>
        /// <param name="isPrincipal">Se é o telefone principal.</param>
        public ProfessionalPhone(Professional professional, Phone phone, bool isPrincipal = false)
        {
            this.Professional = professional ?? throw new ArgumentNullException(nameof(professional));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            ProfessionalId = professional.Id;
            PhoneId = phone.Id;
            IsPrincipal = isPrincipal;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Define este telefone como principal.
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
