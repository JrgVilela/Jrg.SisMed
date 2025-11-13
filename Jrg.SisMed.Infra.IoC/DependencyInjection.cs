using Jrg.SisMed.Application.Factories;
using Jrg.SisMed.Application.Providers;
using Jrg.SisMed.Application.Services.OrganizationServices;
using Jrg.SisMed.Application.UseCases.Organization;
using Jrg.SisMed.Domain.Interfaces.Factories.Professional;
using Jrg.SisMed.Domain.Interfaces.Providers.Professional;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices;
using Jrg.SisMed.Infra.Data.Context;
using Jrg.SisMed.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Registra todas as fábricas concretas
            services.AddTransient<IProfessionalModuleFactory, PsychologyModuleFactory>();
            services.AddTransient<IProfessionalModuleFactory, NutritionModuleFactory>();

            // Registra o provider
            services.AddSingleton<IProfessionalFactoryProvider, ProfessionalModuleFactoryProvider>();

            // Registra Services
            services.AddScoped<ICreateOrganizationService, CreateOrganizationService>();
            services.AddScoped<IUpdateOrganizationService, UpdateOrganizationService>();
            services.AddScoped<IDeleteOrganizationService, DeleteOrganizationService>();
            services.AddScoped<IReadOrganizationService, ReadOrganizationService>();


            // Registra UseCases
            services.AddScoped(typeof(CreateOrganizationUseCase));
            services.AddScoped(typeof(UpdateOrganizationUseCase));
            services.AddScoped(typeof(DeleteOrganizationUseCase));
            services.AddScoped(typeof(ReadOrganizationUseCase));

            // Registra Repositories
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IProfessionalRepository, ProfessionalRepository>();

            return services;
        }
    }
}
