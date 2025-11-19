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
    public class CreateUserValidation : AbstractValidator<User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<Messages> _localizer;
        public CreateUserValidation(IUserRepository userRepository, IStringLocalizer<Messages> localizer)
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
                .MustAsync(async (email, cancellationToken) => !await EmailAlreadyExists(email, cancellationToken))
                .WithMessage(_localizer.For(UserMessage.AlreadyExistsByEmail).Value);

            RuleFor(user => user.Password)
                .NotEmpty()
                .WithMessage(_localizer.For(UserMessage.PasswordRequired).Value)
                .MinimumLength(8)
                .WithMessage(_localizer.For(UserMessage.PasswordMinLength, 8).Value)
                .MaximumLength(25)
                .WithMessage(_localizer.For(UserMessage.PasswordMaxLength, 25).Value);
        }

        /// <summary>
        /// Verifica se o email já está cadastrado no banco de dados.
        /// </summary>
        private async Task<bool> EmailAlreadyExists(string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _userRepository.EmailExistsAsync(email, null, cancellationToken);
        }
    }
}
