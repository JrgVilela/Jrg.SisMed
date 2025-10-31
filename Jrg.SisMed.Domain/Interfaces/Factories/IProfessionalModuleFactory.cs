using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Factories
{
    public interface IProfessionalModuleFactory
    {
        Person CreateProfessional(string name, string cpf, string? rg, DateTime? birthDate, PersonEnum.Gender gender, string email, string password, string crp);
        IAgendaService CreateAgendaService();
        IAppointmentService CreateAppointmentService();
    }
}
