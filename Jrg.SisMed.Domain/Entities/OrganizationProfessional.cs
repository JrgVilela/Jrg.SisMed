using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Represents the association between an organization and a professional.
    /// </summary>
    /// <remarks>This class establishes a relationship between an organization and a professional, identified
    /// by their respective unique identifiers. It is primarily used to model many-to-many relationships in
    /// domain-driven designs.</remarks>
    public class OrganizationProfessional : EntityBase
    {
        public int OrganizationId { get; private set; }
        public virtual Organization Organization { get; private set; } = null!;
        public int ProfessionalId { get; private set; }
        public virtual Professional Professional { get; private set; } = null!;
        public OrganizationProfessionalEnum.State State { get; private set; } = OrganizationProfessionalEnum.State.Active;
        #region Constructors
        protected OrganizationProfessional()
        {
        }
        public OrganizationProfessional(Organization organization, Professional professional)
        {
            Organization = organization;
            Professional = professional;
        }
        #endregion
    }

    public class OrganizationProfessionalEnum
    {
        public enum State
        {
            Active = 1,
            Inactive = 2
        }
    }
}
