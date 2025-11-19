using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Infra.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.IoC
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
