using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class EntityBase : Entity
    {
        public virtual Person CreatedBy { get; protected set; }
        public virtual Person? UpdatedBy { get; protected set; }
    }
}
