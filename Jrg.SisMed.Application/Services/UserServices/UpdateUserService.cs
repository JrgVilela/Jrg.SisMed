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
    /// <summary>
    /// Serviço responsável por atualizar usuários existentes no sistema.
    /// </summary>
    public class UpdateUserService : IUpdateUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateUserService(
            IUserRepository userRepository, 
            IStringLocalizer<Messages> localizer)
        {
            _userRepository = userRepository;
            _localizer = localizer;
        }

        public async Task ExecuteAsync(int id, User user, CancellationToken cancellationToken = default)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), _localizer.For(CommonMessage.ArgumentNull_Generic, nameof(user)));

            // Verifica se o usuário existe
            var currentUser = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (currentUser == null)
                throw new NotFoundException("User", id);

            // Atualiza o usuário
            currentUser.Update(user.Name, user.Email, user.Password, user.State);

            await _userRepository.UpdateAsync(currentUser, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
