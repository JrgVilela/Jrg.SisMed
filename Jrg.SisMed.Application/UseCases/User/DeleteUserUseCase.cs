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
    /// Caso de uso responsável por excluir usuários do sistema.
    /// </summary>
    public class DeleteUserUseCase
    {
        private readonly IDeleteUserService _deleteUserService;
        private readonly IStringLocalizer<Messages> _localizer;

        public DeleteUserUseCase(IDeleteUserService deleteUserService, IStringLocalizer<Messages> localizer)
        {
            _deleteUserService = deleteUserService;
            _localizer = localizer;
        }

        /// <summary>
        /// Executa a exclusão de um usuário.
        /// </summary>
        /// <param name="id">ID do usuário a ser excluído.</param>
        public virtual async Task ExecuteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(_localizer.For(CommonMessage.InvalidArgument), nameof(id));

            await _deleteUserService.ExecuteAsync(id);
        }
    }
}
