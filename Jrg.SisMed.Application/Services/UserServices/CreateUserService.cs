using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Exceptions;
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
    public class CreateUserService : ICreateUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly IStringLocalizer<Messages> _localizer;

        public CreateUserService(IUserRepository userRepository, IStringLocalizer<Messages> localizer)
        {
            _userRepository = userRepository;
            _localizer = localizer;
        }

        public async Task<int> ExecuteAsync(User user, CancellationToken cancellationToken = default)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user), _localizer.For(CommonMessage.ArgumentNull));

            // Verifica se o email já existe
            var existingUser = await _userRepository.FindAsync(u => u.Email.Equals(user.Email), cancellationToken);
            if(existingUser.Any())
                throw new ConflictException("User", "Email", user.Email);

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
