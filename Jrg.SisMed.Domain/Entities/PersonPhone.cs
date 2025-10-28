using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class PersonPhone
    {
        [ForeignKey(nameof(Address))]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; } = new Person();

        [ForeignKey(nameof(Phone))]
        public int PhoneId { get; set; }
        public virtual Phone Phone { get; set; } = new Phone();

        public bool IsPrincipal { get; set; }
    }
}
