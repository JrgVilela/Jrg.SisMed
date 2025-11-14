using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Services.UserServices
{
    public interface ICreateUserService
    {
        Task<int> ExecuteAsync(User user, CancellationToken cancellationToken = default);
    }
}
