using FluentValidation;
using Jrg.SisMed.Application.Validations.UserValidations;
using Jrg.SisMed.Domain.Entities;
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
        private readonly UpdateUserValidation _validator;

        public UpdateUserService(
            IUserRepository userRepository, 
            IStringLocalizer<Messages> localizer,
            UpdateUserValidation validator)
        {
            _userRepository = userRepository;
            _localizer = localizer;
            _validator = validator;
        }

        public async Task ExecuteAsync(int id, User user, CancellationToken cancellationToken = default)
        {
            if (user == null)
                throw new ArgumentNullException(_localizer.For(CommonMessage.ArgumentNull_Generic));

            // Verifica se o usuário existe
            var currentUser = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (currentUser == null)
                throw new KeyNotFoundException(_localizer.For(UserMessage.NotFound));

            // Valida o usuário usando FluentValidation
            // Cria um contexto de validação com o ID do usuário atual
            var validationContext = new ValidationContext<User>(user);
            validationContext.RootContextData["UserId"] = id;
            
            var validationResult = await _validator.ValidateAsync(validationContext, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }

            // Atualiza o usuário
            currentUser.Update(user.Name, user.Email, user.Password, user.State);

            await _userRepository.UpdateAsync(currentUser, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
