using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.DTOs.OrganizationDto
{
    public class UpdateOrganizationDto
    {
        public int Id { get; set; }
        public string NameFantasia { get; private set; } = string.Empty;
        public string RazaoSocial { get; private set; } = string.Empty;
        public string Cnpj { get; private set; } = string.Empty;
        public OrganizationEnum.State State { get; private set; } = OrganizationEnum.State.Active;

        public Organization ToDomainOrganization()
        {
            return new Organization(
                NameFantasia = this.NameFantasia,
                RazaoSocial = this.RazaoSocial,
                Cnpj = this.Cnpj,
                State = this.State
            );
        }
    }
}
