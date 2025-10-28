using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class OrganizationPhone : EntityBase
    {
        [ForeignKey(nameof(Organization))]
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; } = new Organization();

        [ForeignKey(nameof(Phone))]
        public int PhoneId { get; set; }
        public virtual Phone Phone { get; set; } = new Phone();

        public bool IsPrincipal { get; set; }
    }
}