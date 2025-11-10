using Jrg.SisMed.Application.Factories;
using Jrg.SisMed.Application.Providers;
using Jrg.SisMed.Domain.Interfaces.Factories.Person;
using Jrg.SisMed.Domain.Interfaces.Providers.Professional;
using Microsoft.Extensions.DependencyInjection;

namespace Jrg.SisMed.Infra.IoC
{
    /// <summary>
    /// Classe estática para configuração de injeção de dependências.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registra as factories de módulos profissionais e o provider no container de DI.
        /// </summary>
        /// <param name="services">Coleção de serviços.</param>
        /// <returns>Coleção de serviços para encadeamento.</returns>
        public static IServiceCollection AddApplicationFactories(this IServiceCollection services)
        {
            // Registra todas as fábricas concretas
            services.AddTransient<IProfessionalModuleFactory, PsychologyModuleFactory>();
            services.AddTransient<IProfessionalModuleFactory, NutritionModuleFactory>();

            // Registra o provider
            services.AddSingleton<IProfessionalFactoryProvider, ProfessionalModuleFactoryProvider>();

            return services;
        }
    }
}
