using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Classe base para entidades que precisam de auditoria (rastreamento de quem criou/alterou).
    /// </summary>
    public abstract class EntityBase : Entity
    {
        /// <summary>
        /// ID da pessoa que criou esta entidade.
        /// </summary>
        public virtual int? CreatedById { get; protected set; }
        
        /// <summary>
        /// Pessoa que criou esta entidade.
        /// </summary>
        public virtual Person? CreatedBy { get; protected set; }
        
        /// <summary>
        /// ID da pessoa que fez a última atualização nesta entidade.
        /// </summary>
        public virtual int? UpdatedById { get; protected set; }
        
        /// <summary>
        /// Pessoa que fez a última atualização nesta entidade.
        /// </summary>
        public virtual Person? UpdatedBy { get; protected set; }

        /// <summary>
        /// Define quem criou esta entidade.
        /// </summary>
        /// <param name="person">Pessoa que criou a entidade.</param>
        /// <exception cref="ArgumentNullException">Lançado quando person é null.</exception>
        /// <exception cref="InvalidOperationException">Lançado quando CreatedBy já foi definido.</exception>
        public virtual void SetCreatedBy(Person person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person), "A pessoa que criou a entidade não pode ser nula.");
                
            if (CreatedById.HasValue)
                throw new InvalidOperationException("O criador da entidade já foi definido e não pode ser alterado.");
                
            CreatedById = person.Id;
            CreatedBy = person;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Define quem atualizou esta entidade.
        /// </summary>
        /// <param name="person">Pessoa que atualizou a entidade.</param>
        /// <exception cref="ArgumentNullException">Lançado quando person é null.</exception>
        public virtual void SetUpdatedBy(Person person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person), "A pessoa que atualizou a entidade não pode ser nula.");
                
            UpdatedById = person.Id;
            UpdatedBy = person;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
