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
        public string NameFantasia { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string PrincipalDdi { get; set; } = string.Empty;
        public string PrincipalDdd { get; set; } = string.Empty;
        public string PrincipalPhone { get; set; } = string.Empty;
        public string PrincipalPhoneFormatted { get; set; } = string.Empty;
        public string PrincipalPhoneFullFormatted { get; set; } = string.Empty;

        public OrganizationEnum.State State { get; set; } = OrganizationEnum.State.Active;

        public static ReadOrganizationDto FromDomainOrganization(Organization organization)
        {
            Phone? phone = organization.Phones.FirstOrDefault()?.Phone;

            return new ReadOrganizationDto
            {
                Id = organization.Id,
                NameFantasia = organization.NameFantasia,
                RazaoSocial = organization.RazaoSocial,
                Cnpj = organization.Cnpj,
                PrincipalDdi = phone?.Ddi ?? string.Empty,
                PrincipalDdd = phone?.Ddd ?? string.Empty,
                PrincipalPhone = phone?.Number ?? string.Empty,
                PrincipalPhoneFormatted = phone != null ? phone.GetFormattedNumber() : string.Empty,
                PrincipalPhoneFullFormatted = phone != null ? phone.GetFormattedNumber(includeDdi: true) : string.Empty,
                State = organization.State
            };
        }
    }
}
