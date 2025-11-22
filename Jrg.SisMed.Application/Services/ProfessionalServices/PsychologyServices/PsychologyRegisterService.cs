using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Interfaces.Services.ProfessionalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Services.ProfessionalServices.PsychologyServices
{
    public class PsychologyRegisterService : IRegisterService<Psychologist>
    {
        private readonly IProfessionalRepository<Psychologist> _repository;

        public PsychologyRegisterService(IProfessionalRepository<Psychologist> repository)
        {
            _repository = repository;
        }

        public async Task<int> ExecuteAsync(Psychologist professional, CancellationToken cancellationToken = default)
        {
            await _repository.AddAsync(professional, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return professional.Id;
        }
    }
}
