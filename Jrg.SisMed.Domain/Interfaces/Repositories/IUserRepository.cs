using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<Person>
    {
        Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
