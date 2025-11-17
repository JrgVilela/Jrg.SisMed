using FluentValidation;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Interfaces.Repositories;
using Jrg.SisMed.Domain.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Validations.UserValidations
{
    /// <summary>
    /// Validador para atualização de usuários usando FluentValidation.
    /// </summary>
    public class UpdateUserValidation : AbstractValidator<User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateUserValidation(IUserRepository userRepository, IStringLocalizer<Messages> localizer)
        {
            _userRepository = userRepository;
            _localizer = localizer;

            RuleFor(user => user.Name)
                .NotEmpty()
                .WithMessage(_localizer.For(UserMessage.NameRequired).Value)
                .MaximumLength(100)
                .WithMessage(_localizer.For(UserMessage.NameMaxLength, 100).Value);

            RuleFor(user => user.Email)
                .NotEmpty()
                .WithMessage(_localizer.For(UserMessage.EmailRequired).Value)
                .EmailAddress()
                .WithMessage(_localizer.For(UserMessage.EmailInvalid).Value)
                .MaximumLength(100)
                .WithMessage(_localizer.For(UserMessage.EmailMaxLength, 100).Value)
                .MustAsync(async (user, email, context, cancellationToken) =>
                {
                    // Obtém o ID do usuário do contexto de validação
                    var userId = context.RootContextData.TryGetValue("UserId", out var id) ? (int)id : 0;
                    return !await EmailAlreadyExistsByAnotherUser(email, userId, cancellationToken);
                })
                .WithMessage(_localizer.For(UserMessage.EmailAlreadyExists).Value);

            RuleFor(user => user.Password)
                .NotEmpty()
                .WithMessage(_localizer.For(UserMessage.PasswordRequired).Value)
                .MinimumLength(8)
                .WithMessage(_localizer.For(UserMessage.PasswordMinLength, 8).Value)
                .MaximumLength(25)
                .WithMessage(_localizer.For(UserMessage.PasswordMaxLength, 25).Value);

            RuleFor(user => user.State)
                .IsInEnum()
                .WithMessage(_localizer.For(UserMessage.StateInvalid).Value);
        }

        /// <summary>
        /// Verifica se o email já está sendo usado por outro usuário.
        /// </summary>
        private async Task<bool> EmailAlreadyExistsByAnotherUser(string email, int userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _userRepository.EmailExistsAsync(email, excludeUserId: userId, cancellationToken);
        }
    }
}
