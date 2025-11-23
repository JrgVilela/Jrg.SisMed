using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Interfaces.Services.UserServices;
using Jrg.SisMed.Domain.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Services.UserServices
{
    public class LoginUserService : ILoginUserService
    {
        private readonly IUserRepository _userRepository;
        public readonly IStringLocalizer<Messages> _localizer;

        public LoginUserService(IUserRepository userRepository, IStringLocalizer<Messages> localizer)
        {
            _userRepository = userRepository;
            _localizer = localizer;
        }

        public async Task<bool> ExecuteLoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var users =  await _userRepository.FindAsync(u => u.Email.Equals(email.ToLower()));
            if (!users.Any())
                return false; //throw new Exception(_localizer.For(UserMessage.AlreadyExistsByEmail));

            var user = users.First();
            if (!user.VerifyPassword(password))
                return false; //throw new Exception(_localizer.For(UserMessage.LoginFailed));

            return true;
        }
    }
}