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
        public string PrincipalDdi { get; private set; } = string.Empty;
        public string PrincipalDdd { get; private set; } = string.Empty;
        public string PrincipalPhone { get; private set; } = string.Empty;
        public string PrincipalPhoneFormatted { get; private set; } = string.Empty;
        public string PrincipalPhoneFullFormatted { get; private set; } = string.Empty;

        public OrganizationEnum.State State { get; private set; } = OrganizationEnum.State.Active;

        public static ReadOrganizationDto FromDomainOrganization(Organization organization)
        {
            Phone phone = organization.Phones.FirstOrDefault()?.Phone;

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
