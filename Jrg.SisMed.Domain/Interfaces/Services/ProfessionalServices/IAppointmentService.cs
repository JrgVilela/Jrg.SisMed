using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Services.ProfessionalServices
{
    public interface IAppointmentService
    {
        Task PerformAppointment(Guid appointmentId);
    }
}
