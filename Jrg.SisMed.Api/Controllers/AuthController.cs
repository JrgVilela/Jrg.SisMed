using Jrg.SisMed.Application.DTOs.AuthDto;
using Jrg.SisMed.Application.UseCases.AuthUseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jrg.SisMed.Api.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação de usuários.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticateUserUseCase _authenticateUseCase;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthenticateUserUseCase authenticateUseCase,
            ILogger<AuthController> logger)
        {
            _authenticateUseCase = authenticateUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Autentica um usuário e retorna um token JWT.
        /// </summary>
        /// <param name="request">Credenciais de login (email e senha).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Token JWT e informações do usuário.</returns>
        /// <response code="200">Autenticação bem-sucedida.</response>
        /// <response code="400">Dados de entrada inválidos.</response>
        /// <response code="401">Credenciais inválidas ou usuário inativo/bloqueado.</response>
        /// <response code="500">Erro interno do servidor.</response>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST /api/auth/login
        ///     {
        ///         "email": "joao@exemplo.com",
        ///         "password": "SenhaForte@123"
        ///     }
        /// 
        /// Exemplo de resposta:
        /// 
        ///     {
        ///         "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///         "tokenType": "Bearer",
        ///         "expiresIn": 60,
        ///         "user": {
        ///             "id": 1,
        ///             "name": "João Silva",
        ///             "email": "joao@exemplo.com",
        ///             "state": "Active"
        ///         }
        ///     }
        /// 
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Tentativa de login para o e-mail: {Email}", request.Email);

            // Exceções são tratadas pelo ExceptionHandlingMiddleware
            var response = await _authenticateUseCase.ExecuteAsync(request, cancellationToken);

            _logger.LogInformation("Login bem-sucedido para o usuário: {UserId}", response.User.Id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint de teste para verificar se o token JWT é válido.
        /// Requer autenticação.
        /// </summary>
        /// <returns>Informações do usuário autenticado.</returns>
        /// <response code="200">Token válido.</response>
        /// <response code="401">Token inválido ou ausente.</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var userName = User.Identity?.Name;
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            return Ok(new
            {
                Id = userId,
                Name = userName,
                Email = userEmail,
                Message = "Token válido!"
            });
        }
    }
}
