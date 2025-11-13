using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.DTOs.OrganizationDto
{
    public class ReadOrganizationDto
    {
        public int Id { get; set; }
        public string NameFantasia { get; private set; } = string.Empty;
        public string RazaoSocial { get; private set; } = string.Empty;
        public string Cnpj { get; private set; } = string.Empty;
        public string PrincipalPhone { get; set; } = string.Empty;
        public OrganizationEnum.State State { get; private set; } = OrganizationEnum.State.Active;

        public static ReadOrganizationDto FromDomainOrganization(Organization organization)
        {
            return new ReadOrganizationDto
            {
                Id = organization.Id,
                NameFantasia = organization.NameFantasia,
                RazaoSocial = organization.RazaoSocial,
                Cnpj = organization.Cnpj,
                PrincipalPhone = organization.Phones.Any() ? organization.Phones.First().Phone.GetFormattedNumber() : string.Empty,
                State = organization.State
            };
        }
    }
}
