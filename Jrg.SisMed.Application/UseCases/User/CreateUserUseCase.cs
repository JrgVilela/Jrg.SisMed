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
    /// Caso de uso responsável por criar novos usuários no sistema.
    /// </summary>
    public class CreateUserUseCase
    {
        private readonly ICreateUserService _createUserService;
        private readonly IStringLocalizer<Messages> _localizer;

        public CreateUserUseCase(ICreateUserService createUserService, IStringLocalizer<Messages> localizer)
        {
            _createUserService = createUserService;
            _localizer = localizer;
        }

        /// <summary>
        /// Executa a criação de um novo usuário.
        /// </summary>
        /// <param name="userDto">DTO com os dados do usuário a ser criado.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>ID do usuário criado.</returns>
        public virtual async Task<int> ExecuteAsync(CreateUserDto userDto, CancellationToken cancellationToken = default)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto), _localizer.For(CommonMessage.ArgumentNull_Generic, nameof(userDto)));

            var user = userDto.ToDomainUser();
            return await _createUserService.ExecuteAsync(user, cancellationToken);
        }
    }
}
