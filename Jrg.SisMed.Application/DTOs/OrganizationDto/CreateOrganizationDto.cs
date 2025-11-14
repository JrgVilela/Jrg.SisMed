using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.DTOs.OrganizationDto
{
    public class CreateOrganizationDto
    {
        public string NameFantasia { get; private set; } = string.Empty;
        public string RazaoSocial { get; private set; } = string.Empty;
        public string Cnpj { get; private set; } = string.Empty;
        public OrganizationEnum.State State { get; private set; } = OrganizationEnum.State.Active;

        public Organization ToDomainOrganization()
        {
            return new Organization(
                nameFantasia: this.NameFantasia,
                razaoSocial: this.RazaoSocial,
                cnpj: this.Cnpj,
                state: this.State
            );
        }
    }
}