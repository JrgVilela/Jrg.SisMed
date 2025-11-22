using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Resources;
using Jrg.SisMed.Infra.Data.Context;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.Data.Repositories
{
    public class PsychologyRepository : Repository<Psychologist>, IProfessionalRepository<Psychologist>
    {
        public PsychologyRepository(ApplicationDbContext context, IStringLocalizer<Messages> localizer) : base(context, localizer)
        {
        }

        public Task<bool> CpfExistsAsync(string cpf, int? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Psychologist>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Psychologist?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Psychologist?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
