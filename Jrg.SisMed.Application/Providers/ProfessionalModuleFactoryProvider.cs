using Jrg.SisMed.Domain.Attributes;
using Jrg.SisMed.Domain.Enumerators;
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
        private readonly IEnumerable<object> _factories;

        public ProfessionalModuleFactoryProvider(IEnumerable<object> factories)
        {
            _factories = factories;
        }

        public object? GetFactory(ProfessionalType type)
        {
            if (_factories == null || !_factories.Any())
                return null;

            return _factories.First(f =>
             f.GetType().GetCustomAttribute<ProfessionalTypeAttribute>()?.Type == type) ??
             throw new ArgumentException($"Falhar no carregamento do factory informado.");
        }
    }
}
