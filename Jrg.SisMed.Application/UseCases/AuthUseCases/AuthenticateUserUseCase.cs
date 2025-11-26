using Jrg.SisMed.Application.DTOs.AuthDto;
using Jrg.SisMed.Application.Services.AuthServices;
using Jrg.SisMed.Application.UseCases.User;
using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace Jrg.SisMed.Application.UseCases.AuthUseCases
{
    /// <summary>
    /// Caso de uso para autenticação de usuários com JWT.
    /// </summary>
    public class AuthenticateUserUseCase
    {
        private readonly LoginUserUseCase _loginUseCase;
        private readonly ReadUserUseCase _readUserUseCase;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<Messages> _localizer;

        public AuthenticateUserUseCase(
            LoginUserUseCase loginUseCase,
            ReadUserUseCase readUserUseCase,
            JwtTokenService jwtTokenService,
            IConfiguration configuration,
            IStringLocalizer<Messages> localizer)
        {
            _loginUseCase = loginUseCase;
            _readUserUseCase = readUserUseCase;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
            _localizer = localizer;
        }

        /// <summary>
        /// Autentica um usuário e gera um token JWT.
        /// </summary>
        /// <param name="request">Dados de login (email e senha).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Token JWT e informações do usuário.</returns>
        /// <exception cref="UnauthorizedException">Lançada quando as credenciais são inválidas.</exception>
        public async Task<LoginResponseDto> ExecuteAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken = default)
        {
            // 1. Valida as credenciais
            var isValidLogin = await _loginUseCase.ExecuteAsync(
                request.Email,
                request.Password,
                cancellationToken);

            if (!isValidLogin)
            {
                throw new UnauthorizedException(_localizer.For(UserMessage.LoginFailed));
            }

            // 2. Busca os dados completos do usuário
            var user = await _readUserUseCase.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedException(_localizer.For(UserMessage.LoginFailed));
            }

            // 3. Verifica se o usuário está ativo
            if (user.State != Domain.Entities.UserEnum.State.Active)
            {
                throw new UnauthorizedException(_localizer.For(UserMessage.LoginFailed));
            }

            // 4. Gera o token JWT
            var token = _jwtTokenService.GenerateToken(user);

            // 5. Obtém o tempo de expiração da configuração
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            // 6. Monta a resposta
            return new LoginResponseDto
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresIn = expirationMinutes,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    State = user.State.ToString()
                }
            };
        }
    }
}
