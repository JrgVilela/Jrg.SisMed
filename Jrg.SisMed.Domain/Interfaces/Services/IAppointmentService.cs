using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Services
{
    public interface IAppointmentService
    {
        void PerformAppointment(Guid appointmentId);
    }
}
