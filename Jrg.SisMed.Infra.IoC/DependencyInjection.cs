using FluentValidation;
using Jrg.SisMed.Application.Factories;
using Jrg.SisMed.Application.Providers;
using Jrg.SisMed.Application.Services.OrganizationServices;
using Jrg.SisMed.Application.Services.ProfessionalServices.PsychologyServices;
using Jrg.SisMed.Application.Services.UserServices;
using Jrg.SisMed.Application.UseCases.Organization;
using Jrg.SisMed.Application.UseCases.User;
using Jrg.SisMed.Application.Validations.UserValidations;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Factories.ProfessionalFactories;
using Jrg.SisMed.Domain.Interfaces.Providers.ProfessionalProviders;
using Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices;
using Jrg.SisMed.Domain.Interfaces.Services.ProfessionalServices;
using Jrg.SisMed.Domain.Interfaces.Services.UserServices;
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
        /// Registra toda a infraestrutura no container de DI.
        /// </summary>
        /// <param name="services">Coleção de serviços.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        /// <returns>Coleção de serviços para encadeamento.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Professional Factories
            services.AddScoped<IProfessionalModuleFactory, PsychologyModuleFactory>();
            
            // Professional Factory Provider
            services.AddScoped<IProfessionalFactoryProvider, ProfessionalModuleFactoryProvider>();

            // Registra FluentValidation - registra TODOS os validators do assembly automaticamente
            services.AddValidatorsFromAssemblyContaining<CreateUserValidation>();

            //Injeta todos os services
            services.AddServices();

            //Injeta todos os use cases
            services.AddUseCases();

            //Injeto todos os repositories
            services.AddRepositories();


            return services;
        }
    }
}
