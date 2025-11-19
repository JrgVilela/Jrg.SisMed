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
    public class CreateUserService : ICreateUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly CreateUserValidation _validator;
        public readonly IStringLocalizer<Messages> _localizer;

        public CreateUserService(IUserRepository userRepository, CreateUserValidation validator, IStringLocalizer<Messages> localizer)
        {
            _userRepository = userRepository;
            _validator = validator;
            _localizer = localizer;
        }

        public async Task<int> ExecuteAsync(User user, CancellationToken cancellationToken = default)
        {
            if(user == null)
                throw new ArgumentNullException(_localizer.For(CommonMessage.ArgumentNull));

            // Valida o usuário (incluindo verificação de email duplicado)
            var validationResult = await _validator.ValidateAsync(user, cancellationToken);

            if (!validationResult.IsValid)
            {
                // Lança exceção com todas as mensagens de erro
                throw new ValidationException(validationResult.Errors);
            }

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
