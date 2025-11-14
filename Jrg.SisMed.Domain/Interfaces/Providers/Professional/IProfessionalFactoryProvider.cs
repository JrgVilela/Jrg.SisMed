using Jrg.SisMed.Domain.Enumerators;
using Jrg.SisMed.Domain.Interfaces.Factories.Professional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Providers.Professional
{
    public interface IProfessionalFactoryProvider
    {
        IProfessionalModuleFactory GetFactory(ProfessionalType type);
    }
}
