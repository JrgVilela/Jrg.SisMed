using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class PersonAddress : EntityBase
    {
        [ForeignKey(nameof(Address))]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; } = new Person();


        [ForeignKey(nameof(Address))]
        public int AddreId { get; set; }
        public virtual Address Address { get; set; } = new Address();

        public bool IsPrincipal { get; set; }
    }
}
