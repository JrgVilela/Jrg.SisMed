using Jrg.SisMed.Application.Services.AuthServices;
using Jrg.SisMed.Application.Services.OrganizationServices;
using Jrg.SisMed.Application.Services.ProfessionalServices.PsychologyServices;
using Jrg.SisMed.Application.Services.UserServices;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Services.OrganizationServices;
using Jrg.SisMed.Domain.Interfaces.Services.ProfessionalServices;
using Jrg.SisMed.Domain.Interfaces.Services.UserServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.IoC
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Organization Services
            services.AddScoped<ICreateOrganizationService, CreateOrganizationService>();
            services.AddScoped<IUpdateOrganizationService, UpdateOrganizationService>();
            services.AddScoped<IDeleteOrganizationService, DeleteOrganizationService>();
            services.AddScoped<IReadOrganizationService, ReadOrganizationService>();

            // Professional Services
            services.AddScoped<IRegisterService<Psychologist>, PsychologyRegisterService>();

            // User Services
            services.AddScoped<ICreateUserService, CreateUserService>();
            services.AddScoped<IUpdateUserService, UpdateUserService>();
            services.AddScoped<IReadUserService, ReadUserService>();
            services.AddScoped<IDeleteUserService, DeleteUserService>();
            services.AddScoped<ILoginUserService, LoginUserService>();

            // Auth Services
            services.AddScoped<JwtTokenService>();

            return services;
        }
    }
}
