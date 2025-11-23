using Jrg.SisMed.Domain.Interfaces.Services.UserServices;
using Jrg.SisMed.Domain.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.UseCases.User
{
    public class LoginUserUseCase
    {
        private readonly ILoginUserService _loginUserService;
        private readonly IStringLocalizer<Messages> _localizer;
        public LoginUserUseCase(ILoginUserService loginUserService, IStringLocalizer<Messages> localizer)
        {
            _loginUserService = loginUserService;
            _localizer = localizer;
        }

        public async Task<bool> ExecuteAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(_localizer.For(UserMessage.EmailRequired));

            if(string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(_localizer.For(UserMessage.PasswordRequired));

            return await _loginUserService.ExecuteLoginAsync(email, password, cancellationToken);
        }
    }
}