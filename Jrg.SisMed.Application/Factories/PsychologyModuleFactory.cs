using Jrg.SisMed.Application.Services;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Factories;
using Jrg.SisMed.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Factories
{
    public class PsychologyModuleFactory : IProfessionalModuleFactory
    {

        private readonly IAgendaService _agendaService;
        private readonly IAppointmentService _appointmentService;

        public PsychologyModuleFactory(IAgendaService agendaService, IAppointmentService appointmentService)
        {
            _agendaService = agendaService;
            _appointmentService = appointmentService;
        }

        public Person CreateProfessional(string name, string cpf, string? rg, DateTime? birthDate, PersonEnum.Gender gender, string email, string password, string crp)
            => new Psychologist(name, cpf, rg, birthDate, gender, email, password, crp);

        public IAgendaService CreateAgendaService() => _agendaService;

        public IAppointmentService CreateAppointmentService() => _appointmentService;

    }
}
