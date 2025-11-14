using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Representa o relacionamento entre Professional e Address.
    /// </summary>
    public class ProfessionalAddress : EntityBase
    {
        /// <summary>
        /// ID da pessoa.
        /// </summary>
        [ForeignKey(nameof(Professional))]
        public int ProfessionalId { get; set; }
        
        /// <summary>
        /// Navegação para a pessoa.
        /// </summary>
        public virtual Professional Professional { get; set; } = null!;

        /// <summary>
        /// ID do endereço.
        /// </summary>
        [ForeignKey(nameof(Address))]
        public int AddressId { get; set; }
        
        /// <summary>
        /// Navegação para o endereço.
        /// </summary>
        public virtual Address Address { get; set; } = null!;

        /// <summary>
        /// Indica se este é o endereço principal da pessoa.
        /// </summary>
        public bool IsPrincipal { get; private set; }

        #region Constructors
        /// <summary>
        /// Construtor protegido para uso do Entity Framework.
        /// </summary>
        internal ProfessionalAddress() { }

        /// <summary>
        /// Cria uma nova relação entre pessoa e endereço.
        /// </summary>
        /// <param name="professional">Pessoa dona do endereço.</param>
        /// <param name="address">Endereço a ser associado.</param>
        /// <param name="isPrincipal">Se é o endereço principal.</param>
        public ProfessionalAddress(Professional professional, Address address, bool isPrincipal = false)
        {
            Professional = professional ?? throw new ArgumentNullException(nameof(professional));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            ProfessionalId = professional.Id;
            AddressId = address.Id;
            IsPrincipal = isPrincipal;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Define este endereço como principal.
        /// </summary>
        public void SetAsPrincipal()
        {
            IsPrincipal = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Define este endereço como secundário.
        /// </summary>
        public void SetAsSecondary()
        {
            IsPrincipal = false;
            UpdatedAt = DateTime.UtcNow;
        }
        #endregion
    }
}
