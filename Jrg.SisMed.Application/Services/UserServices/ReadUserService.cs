using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Helpers;
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
    /// <summary>
    /// Serviço responsável por operações de leitura de usuários.
    /// </summary>
    public class ReadUserService : IReadUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public ReadUserService(IUserRepository userRepository, IStringLocalizer<Messages> localizer)
        {
            _userRepository = userRepository;
            _localizer = localizer;
        }

        public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _userRepository.ExistByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetAllAsync(cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (email.IsNullOrEmpty())
                throw new ArgumentException(_localizer.For(UserMessage.EmailRequired), nameof(email));

            if (!email.IsEmail())
                throw new ArgumentException(_localizer.For(UserMessage.EmailInvalid, email));

            var result = await _userRepository.FindAsync(u => u.Email.Equals(email), cancellationToken);

            if (result.Count() > 1)
                throw new InvalidOperationException(_localizer.For(UserMessage.MultipleUsersFound, email));

            return result.FirstOrDefault();
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetByIdAsync(id, cancellationToken);
        }
    }
}
