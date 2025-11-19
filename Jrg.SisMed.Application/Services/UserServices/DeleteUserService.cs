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
    /// Serviço responsável por exclusão de usuários do sistema.
    /// </summary>
    public class DeleteUserService : IDeleteUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public DeleteUserService(IUserRepository userRepository, IStringLocalizer<Messages> localizer)
        {
            _userRepository = userRepository;
            _localizer = localizer;
        }

        public async Task ExecuteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User", id);

            await _userRepository.RemoveAsync(id);
            await _userRepository.SaveChangesAsync();
        }
    }
}
