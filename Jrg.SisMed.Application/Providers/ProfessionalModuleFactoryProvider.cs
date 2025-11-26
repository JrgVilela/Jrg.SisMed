using Jrg.SisMed.Domain.Attributes;
using Jrg.SisMed.Domain.Enumerators;
using Jrg.SisMed.Domain.Interfaces.Factories.ProfessionalFactories;
using Jrg.SisMed.Domain.Interfaces.Providers.ProfessionalProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Providers
{
    public class ProfessionalModuleFactoryProvider : IProfessionalFactoryProvider
    {
        private readonly IReadOnlyDictionary<ProfessionalType, IProfessionalModuleFactory> _factories;

        public ProfessionalModuleFactoryProvider(IEnumerable<IProfessionalModuleFactory> factories)
        {
            _factories = factories.ToDictionary(f => f.Type);
        }

        public IProfessionalModuleFactory GetFactory(ProfessionalType type)
        {
            if (_factories.TryGetValue(type, out var factory))
                return factory;

            throw new InvalidOperationException(
                $"No professional module factory registered for type '{type}'.");
        }
    }
}
