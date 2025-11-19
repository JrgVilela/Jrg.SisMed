using Jrg.SisMed.Application.DTOs.UserDto;
using Jrg.SisMed.Domain.Interfaces.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.UseCases.User
{
    /// <summary>
    /// Caso de uso responsável por operações de leitura de usuários.
    /// </summary>
    public class ReadUserUseCase
    {
        private readonly IReadUserService _readUserService;

        public ReadUserUseCase(IReadUserService readUserService)
        {
            _readUserService = readUserService;
        }

        /// <summary>
        /// Verifica se um usuário existe pelo ID.
        /// </summary>
        public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _readUserService.ExistsByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Obtém todos os usuários.
        /// </summary>
        public async Task<IEnumerable<ReadUserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _readUserService.GetAllAsync(cancellationToken);

            if (result.Any() is false)
                return new List<ReadUserDto>(); // Retorna lista vazia

            return result.Select(u => ReadUserDto.FromDomainUser(u));
        }

        /// <summary>
        /// Obtém um usuário por email.
        /// </summary>
        public async Task<ReadUserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var result = await _readUserService.GetByEmailAsync(email, cancellationToken);

            if (result is null)
                return null;

            return ReadUserDto.FromDomainUser(result);
        }

        /// <summary>
        /// Obtém um usuário por ID.
        /// </summary>
        public async Task<ReadUserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await _readUserService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return null;

            return ReadUserDto.FromDomainUser(result);
        }
    }
}
