using Jrg.SisMed.Application.DTOs.UserDto;
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
    /// <summary>
    /// Caso de uso responsável por atualizar usuários existentes no sistema.
    /// </summary>
    public class UpdateUserUseCase
    {
        private readonly IUpdateUserService _updateUserService;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateUserUseCase(IUpdateUserService updateUserService, IStringLocalizer<Messages> localizer)
        {
            _updateUserService = updateUserService;
            _localizer = localizer;
        }

        /// <summary>
        /// Executa a atualização de um usuário existente.
        /// </summary>
        /// <param name="id">ID do usuário a ser atualizado.</param>
        /// <param name="userDto">DTO com os novos dados do usuário.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        public virtual async Task ExecuteAsync(int id, UpdateUserDto userDto, CancellationToken cancellationToken = default)
        {
            if (id <= 0 || userDto == null)
                throw new ArgumentNullException(nameof(userDto), _localizer.For(CommonMessage.ArgumentNull));

            var user = userDto.ToDomainUser();

            await _updateUserService.ExecuteAsync(id, user, cancellationToken);
        }
    }
}
