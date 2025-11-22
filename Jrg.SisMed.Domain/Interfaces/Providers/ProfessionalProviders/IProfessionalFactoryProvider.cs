using Jrg.SisMed.Domain.Enumerators;
using Jrg.SisMed.Domain.Interfaces.Factories.ProfessionalFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Providers.ProfessionalProviders
{
    public interface IProfessionalFactoryProvider
    {
        object GetFactory(ProfessionalType type);
    }
}
