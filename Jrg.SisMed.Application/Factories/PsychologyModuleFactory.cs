using Jrg.SisMed.Application.DTOs;
using Jrg.SisMed.Domain.Attributes;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Enumerators;
using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Interfaces.Factories.ProfessionalFactories;
using Jrg.SisMed.Domain.Interfaces.Services.ProfessionalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Factories
{
    /// <summary>
    /// Factory responsável por criar instâncias de profissionais de psicologia.
    /// Esta implementação na Application Layer usa DTOs para receber dados estruturados.
    /// </summary>
    [ProfessionalType(ProfessionalType.Psychologist)]
    public class PsychologyModuleFactory : IProfessionalModuleFactory<Psychologist>
    {
        private readonly IRegisterService<Psychologist> _registerService;

        public PsychologyModuleFactory(IRegisterService<Psychologist> registerService)
        {
            _registerService = registerService;
        }


        public IRegisterService<Psychologist> CreateRegister()
        {
            return _registerService;
        }
    }
}
