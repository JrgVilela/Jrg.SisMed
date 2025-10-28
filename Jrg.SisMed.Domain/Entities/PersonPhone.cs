using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Representa o relacionamento entre Person e Phone.
    /// </summary>
    public class PersonPhone : EntityBase
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
        /// ID do telefone.
        /// </summary>
        [ForeignKey(nameof(Phone))]
        public int PhoneId { get; set; }
        
        /// <summary>
        /// Navegação para o telefone.
        /// </summary>
        public virtual Phone Phone { get; set; } = null!;

        /// <summary>
        /// Indica se este é o telefone principal da pessoa.
        /// </summary>
        public bool IsPrincipal { get; private set; }

        #region Constructors
        /// <summary>
        /// Construtor protegido para uso do Entity Framework.
        /// </summary>
        internal PersonPhone() { }

        /// <summary>
        /// Cria uma nova relação entre pessoa e telefone.
        /// </summary>
        /// <param name="person">Pessoa dona do telefone.</param>
        /// <param name="phone">Telefone a ser associado.</param>
        /// <param name="isPrincipal">Se é o telefone principal.</param>
        public PersonPhone(Person person, Phone phone, bool isPrincipal = false)
        {
            Person = person ?? throw new ArgumentNullException(nameof(person));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            PersonId = person.Id;
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
