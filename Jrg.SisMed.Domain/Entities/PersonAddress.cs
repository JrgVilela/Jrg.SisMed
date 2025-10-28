using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Representa o relacionamento entre Person e Address.
    /// </summary>
    public class PersonAddress : EntityBase
    {
        /// <summary>
        /// ID da pessoa.
        /// </summary>
        [ForeignKey(nameof(Person))]
        public int PersonId { get; set; }
        
        /// <summary>
        /// Navegação para a pessoa.
        /// </summary>
        public virtual Person Person { get; set; } = null!;

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
        internal PersonAddress() { }

        /// <summary>
        /// Cria uma nova relação entre pessoa e endereço.
        /// </summary>
        /// <param name="person">Pessoa dona do endereço.</param>
        /// <param name="address">Endereço a ser associado.</param>
        /// <param name="isPrincipal">Se é o endereço principal.</param>
        public PersonAddress(Person person, Address address, bool isPrincipal = false)
        {
            Person = person ?? throw new ArgumentNullException(nameof(person));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            PersonId = person.Id;
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
