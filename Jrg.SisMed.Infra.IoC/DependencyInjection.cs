using Jrg.SisMed.Application.Factories;
using Jrg.SisMed.Application.Providers;
using Jrg.SisMed.Domain.Interfaces.Factories;
using Jrg.SisMed.Domain.Interfaces.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationFactories(this IServiceCollection services)
        {
            // Registra todas as fábricas concretas
            services.AddTransient<IProfessionalModuleFactory, PsychologyModuleFactory>();
            //services.AddTransient<IProfessionalModuleFactory, NutritionModuleFactory>();

            // Registra o provider
            services.AddSingleton<IProfessionalFactoryProvider, ProfessionalModuleFactoryProvider>();

            return services;
        }
    }
}
