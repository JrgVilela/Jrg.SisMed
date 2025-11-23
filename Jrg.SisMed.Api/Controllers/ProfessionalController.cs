using Jrg.SisMed.Application.DTOs.ProfessionalDto;
using Jrg.SisMed.Application.UseCases.ProfessionalUseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jrg.SisMed.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelas operações relacionadas a profissionais.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProfessionalController : ControllerBase
    {
        private readonly RegisterProfessionalUseCase _registerUseCase;
        private readonly ILogger<ProfessionalController> _logger;

        public ProfessionalController(
            RegisterProfessionalUseCase registerUseCase,
            ILogger<ProfessionalController> logger)
        {
            _registerUseCase = registerUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo profissional no sistema.
        /// </summary>
        /// <param name="registerDto">Dados do profissional a ser registrado.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>ID do profissional criado.</returns>
        /// <response code="201">Profissional registrado com sucesso.</response>
        /// <response code="400">Dados de entrada inválidos (erros de validação).</response>
        /// <response code="409">Conflito - CPF, Email, CRP, CNPJ ou Razão Social já existem.</response>
        /// <response code="500">Erro interno do servidor.</response>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST /api/professional/register
        ///     {
        ///         "name": "Dr. João Silva",
        ///         "cpf": "123.456.789-00",
        ///         "registerNumber": "06/12345",
        ///         "professionalType": "Psychologist",
        ///         "email": "joao.silva@exemplo.com",
        ///         "password": "SenhaForte@123",
        ///         "phone": "+55 (11) 98399-1005",
        ///         "street": "Rua das Flores",
        ///         "number": "123",
        ///         "complement": "Apto 45",
        ///         "neighborhood": "Centro",
        ///         "zipCode": "01234567",
        ///         "city": "São Paulo",
        ///         "state": "SP",
        ///         "razaoSocial": "Clínica de Psicologia LTDA",
        ///         "nomeFantasia": "Clínica Mente Sã",
        ///         "cnpj": "12.345.678/0001-95"
        ///     }
        /// 
        /// </remarks>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDto registerDto,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Iniciando registro de profissional: {ProfessionalType}", registerDto.ProfessionalType);

            // Executa o use case de registro
            // Exceções são tratadas pelo ExceptionHandlingMiddleware
            var professionalId = await _registerUseCase.ExecuteAsync(registerDto);

            _logger.LogInformation("Profissional registrado com sucesso. ID: {ProfessionalId}", professionalId);

            var response = new RegisterResponseDto
            {
                Id = professionalId,
                Message = "Profissional registrado com sucesso."
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = professionalId },
                response);
        }

        /// <summary>
        /// Obtém um profissional pelo ID.
        /// </summary>
        /// <param name="id">ID do profissional.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Dados do profissional.</returns>
        /// <response code="200">Profissional encontrado.</response>
        /// <response code="404">Profissional não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            [FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implementar GetByIdUseCase
            _logger.LogInformation("Buscando profissional por ID: {ProfessionalId}", id);

            return Ok(new
            {
                Id = id,
                Message = "Endpoint em desenvolvimento"
            });
        }
    }

    #region Response DTOs

    /// <summary>
    /// Resposta de sucesso no registro de profissional.
    /// </summary>
    public class RegisterResponseDto
    {
        /// <summary>
        /// ID do profissional criado.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Mensagem de sucesso.
        /// </summary>
        /// <example>Profissional registrado com sucesso.</example>
        public string Message { get; set; } = string.Empty;
    }

    #endregion
}
