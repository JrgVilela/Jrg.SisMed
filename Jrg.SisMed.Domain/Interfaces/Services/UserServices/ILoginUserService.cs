using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Interfaces.Services.UserServices
{
    public interface ILoginUserService
    {
        public Task<bool> ExecuteLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    }
}
