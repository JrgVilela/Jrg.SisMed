using Jrg.SisMed.Application.DTOs.UserDto;
using Jrg.SisMed.Application.UseCases.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Jrg.SisMed.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelas operações CRUD de usuários.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CreateUserUseCase _createUserUseCase;
        private readonly UpdateUserUseCase _updateUserUseCase;
        private readonly ReadUserUseCase _readUserUseCase;
        private readonly DeleteUserUseCase _deleteUserUseCase;

        public UserController(
            CreateUserUseCase createUserUseCase,
            UpdateUserUseCase updateUserUseCase,
            ReadUserUseCase readUserUseCase,
            DeleteUserUseCase deleteUserUseCase)
        {
            _createUserUseCase = createUserUseCase;
            _updateUserUseCase = updateUserUseCase;
            _readUserUseCase = readUserUseCase;
            _deleteUserUseCase = deleteUserUseCase;
        }

        /// <summary>
        /// Obtém todos os usuários.
        /// </summary>
        /// <response code="200">Retorna a lista de usuários</response>
        /// <response code="404">Nenhum usuário encontrado</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _readUserUseCase.GetAllAsync(cancellationToken);

            if (!result.Any())
                return NotFound("No users found.");

            return Ok(result);
        }

        /// <summary>
        /// Obtém um usuário por ID.
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <response code="200">Retorna o usuário encontrado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _readUserUseCase.GetByIdAsync(id, cancellationToken);

            if (result == null)
                return NotFound($"User with id {id} not found.");

            return Ok(result);
        }

        /// <summary>
        /// Busca usuário por email usando query string.
        /// Exemplo: GET /api/user/search?email=usuario@email.com
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <response code="200">Retorna o usuário encontrado</response>
        /// <response code="400">Email não informado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchByEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required");

            var result = await _readUserUseCase.GetByEmailAsync(email, cancellationToken);

            if (result == null)
                return NotFound($"User with email {email} not found.");

            return Ok(result);
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="createUserDto">Dados do usuário a ser criado</param>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            var result = await _createUserUseCase.ExecuteAsync(createUserDto, cancellationToken);

            if (result <= 0)
                return BadRequest("Failed to create user.");

            return CreatedAtAction(nameof(GetById), new { id = result }, result);
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="id">ID do usuário a ser atualizado</param>
        /// <param name="updateUserDto">Novos dados do usuário</param>
        /// <response code="200">Usuário atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto updateUserDto, CancellationToken cancellationToken)
        {
            await _updateUserUseCase.ExecuteAsync(id, updateUserDto, cancellationToken);

            return Ok(new { message = "User updated successfully" });
        }

        /// <summary>
        /// Exclui um usuário.
        /// </summary>
        /// <param name="id">ID do usuário a ser excluído</param>
        /// <response code="204">Usuário excluído com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _deleteUserUseCase.ExecuteAsync(id);
            return NoContent();
        }
    }
}
