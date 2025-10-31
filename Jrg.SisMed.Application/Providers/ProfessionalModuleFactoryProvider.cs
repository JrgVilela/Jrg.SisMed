using Jrg.SisMed.Application.Factories;
using Jrg.SisMed.Domain.Enumerators;
using Jrg.SisMed.Domain.Interfaces.Factories;
using Jrg.SisMed.Domain.Interfaces.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Providers
{
    public class ProfessionalModuleFactoryProvider : IProfessionalFactoryProvider
    {


        //public static IProfessionalModuleFactory GetFactory(ProfessionalType type)
        //{
        //    return type switch
        //    {
        //        ProfessionalType.Psychologist => new PsychologyModuleFactory(),
        //        //ProfessionalType.Nutritionist => new NutritionModuleFactory(),
        //        _ => throw new ArgumentException("Tipo de módulo inválido")
        //    };
        //}

        private readonly Dictionary<ProfessionalType, IProfessionalModuleFactory> _factories;

        public ProfessionalModuleFactoryProvider(IEnumerable<IProfessionalModuleFactory> factories)
        {
            _factories = factories.ToDictionary(f => GetTypeFromFactory(f));
        }

        private static ProfessionalType GetTypeFromFactory(IProfessionalModuleFactory factory)
        {
            return factory switch
            {
                PsychologyModuleFactory => ProfessionalType.Psychologist,
                //NutritionModuleFactory => ProfessionalType.Nutritionist,
                _ => throw new ArgumentException("Tipo de módulo desconhecido.")
            };
        }

        public IProfessionalModuleFactory GetFactory(ProfessionalType type)
        {
            if (_factories.TryGetValue(type, out var factory))
                return factory;

            throw new InvalidOperationException($"Nenhuma fábrica registrada para {type}");
        }
    }
}
