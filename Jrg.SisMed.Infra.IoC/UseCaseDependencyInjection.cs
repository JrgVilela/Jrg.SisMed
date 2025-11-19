using Jrg.SisMed.Application.UseCases.Organization;
using Jrg.SisMed.Application.UseCases.User;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.IoC
{
    public static class UseCaseDependencyInjection
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
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

            return services;
        }
    }
}
