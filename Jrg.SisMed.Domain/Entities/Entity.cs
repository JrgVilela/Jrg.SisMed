using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Classe base para todas as entidades do domínio.
    /// Fornece identificador único e timestamps de auditoria.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Identificador único da entidade.
        /// </summary>
        public virtual int Id { get; protected set; }
        
        /// <summary>
        /// Data e hora de criação da entidade (UTC).
        /// </summary>
        public virtual DateTime CreatedAt { get; protected set; }
        
        /// <summary>
        /// Data e hora da última atualização da entidade (UTC).
        /// </summary>
        public virtual DateTime? UpdatedAt { get; protected set; }

        /// <summary>
        /// Construtor protegido para garantir que entidades sejam criadas adequadamente.
        /// </summary>
        protected Entity()
        {
        }

        /// <summary>
        /// Verifica se a entidade foi persistida no banco de dados.
        /// </summary>
        public bool IsTransient() => Id == 0;

        public void SetUpdatedAt()
        {
            if(!IsTransient())
                UpdatedAt = DateTime.UtcNow;
        }

        public void SetCreatedAt()
        {
            if(IsTransient())
                CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Verifica igualdade entre entidades baseada no ID.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (IsTransient() || other.IsTransient())
                return false;

            return Id == other.Id;
        }

        /// <summary>
        /// Operador de igualdade.
        /// </summary>
        public static bool operator ==(Entity? left, Entity? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Operador de desigualdade.
        /// </summary>
        public static bool operator !=(Entity? left, Entity? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Obtém o hash code baseado no ID da entidade.
        /// </summary>
        public override int GetHashCode()
        {
            return (GetType().ToString() + Id).GetHashCode();
        }
    }
}
