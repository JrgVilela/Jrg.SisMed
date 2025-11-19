using FluentValidation;
using Jrg.SisMed.Application.Factories;
using Jrg.SisMed.Application.Providers;
using Jrg.SisMed.Application.Services.OrganizationServices;
using Jrg.SisMed.Application.Services.UserServices;
using Jrg.SisMed.Application.UseCases.Organization;
using Jrg.SisMed.Application.UseCases.User;
using Jrg.SisMed.Application.Validations.UserValidations;
using Jrg.SisMed.Domain.Interfaces.Factories.Professional;
using Jrg.SisMed.Domain.Interfaces.Providers.Professional;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices;
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
            services.AddTransient<IProfessionalModuleFactory, PsychologyModuleFactory>();
            services.AddTransient<IProfessionalModuleFactory, NutritionModuleFactory>();

            // Professional Factory Provider
            services.AddSingleton<IProfessionalFactoryProvider, ProfessionalModuleFactoryProvider>();

            // Organization Services
            services.AddScoped<ICreateOrganizationService, CreateOrganizationService>();
            services.AddScoped<IUpdateOrganizationService, UpdateOrganizationService>();
            services.AddScoped<IDeleteOrganizationService, DeleteOrganizationService>();
            services.AddScoped<IReadOrganizationService, ReadOrganizationService>();

            // User Services
            services.AddScoped<ICreateUserService, CreateUserService>();
            services.AddScoped<IUpdateUserService, UpdateUserService>();
            services.AddScoped<IReadUserService, ReadUserService>();
            services.AddScoped<IDeleteUserService, DeleteUserService>();

            // Organization UseCases
            services.AddScoped<CreateOrganizationUseCase>();
            services.AddScoped<UpdateOrganizationUseCase>();
            services.AddScoped<DeleteOrganizationUseCase>();
            services.AddScoped<ReadOrganizationUseCase>();

            // User UseCases
            services.AddScoped<CreateUserUseCase>();
            services.AddScoped<UpdateUserUseCase>();
            services.AddScoped<ReadUserUseCase>();
            services.AddScoped<DeleteUserUseCase>();

            // Repositories
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Registra FluentValidation - registra TODOS os validators do assembly automaticamente
            services.AddValidatorsFromAssemblyContaining<CreateUserValidation>();

            return services;
        }
    }
}
