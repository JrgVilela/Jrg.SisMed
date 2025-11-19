using Jrg.SisMed.Application.DTOs.OrganizationDto;
using Jrg.SisMed.Application.UseCases.Organization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web;

namespace Jrg.SisMed.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelas operações CRUD de organizações.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizacaoController : ControllerBase
    {
        private readonly CreateOrganizationUseCase _createOrganizationUseCase;
        private readonly UpdateOrganizationUseCase _updateOrganizationUseCase;
        private readonly ReadOrganizationUseCase _readOrganizationUseCase;
        private readonly DeleteOrganizationUseCase _deleteOrganizationUseCase;

        public OrganizacaoController(
            CreateOrganizationUseCase createOrganizationUseCase,
            UpdateOrganizationUseCase updateOrganizationUseCase,
            ReadOrganizationUseCase readOrganizationUseCase,
            DeleteOrganizationUseCase deleteOrganizationUseCase)
        {
            _createOrganizationUseCase = createOrganizationUseCase;
            _updateOrganizationUseCase = updateOrganizationUseCase;
            _readOrganizationUseCase = readOrganizationUseCase;
            _deleteOrganizationUseCase = deleteOrganizationUseCase;
        }

        /// <summary>
        /// Obtém todas as organizações.
        /// </summary>
        /// <response code="200">Retorna a lista de organizações</response>
        /// <response code="404">Nenhuma organização encontrada</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _readOrganizationUseCase.GetAllAsync(cancellationToken);

            if(!result.Any())
                return NotFound(new { message = "No organizations found." });

            return Ok(result);
        }

        /// <summary>
        /// Obtém uma organização por ID.
        /// </summary>
        /// <param name="id">ID da organização</param>
        /// <response code="200">Retorna a organização encontrada</response>
        /// <response code="404">Organização não encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var result = await _readOrganizationUseCase.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Busca organização por CNPJ usando query string.
        /// Exemplo: GET /api/organizacao/search?cnpj=43.133.410/0001-13
        /// </summary>
        /// <param name="cnpj">CNPJ com ou sem formatação</param>
        /// <response code="200">Retorna a organização encontrada</response>
        /// <response code="400">CNPJ não informado ou inválido</response>
        /// <response code="404">Organização não encontrada</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Search([FromQuery] string cnpj, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return BadRequest(new { message = "CNPJ is required" });

            var result = await _readOrganizationUseCase.GetByCnpjAsync(cnpj, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Cria uma nova organização.
        /// </summary>
        /// <param name="createOrganizationDto">Dados da organização a ser criada</param>
        /// <response code="201">Organização criada com sucesso</response>
        /// <response code="400">Dados inválidos ou violação de regras de negócio</response>
        /// <response code="409">Organização já existe (conflito)</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] CreateOrganizationDto createOrganizationDto, CancellationToken cancellationToken)
        {
            var result = await _createOrganizationUseCase.ExecuteAsync(createOrganizationDto, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result }, new { id = result, message = "Organization created successfully" });
        }

        /// <summary>
        /// Atualiza uma organização existente.
        /// </summary>
        /// <param name="id">ID da organização a ser atualizada</param>
        /// <param name="updateOrganizationDto">Novos dados da organização</param>
        /// <response code="200">Organização atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Organização não encontrada</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateOrganizationDto updateOrganizationDto, CancellationToken cancellationToken)
        {
            await _updateOrganizationUseCase.ExecuteAsync(id, updateOrganizationDto, cancellationToken);
            return Ok(new { message = "Organization updated successfully" });
        }

        /// <summary>
        /// Exclui uma organização.
        /// </summary>
        /// <param name="id">ID da organização a ser excluída</param>
        /// <response code="204">Organização excluída com sucesso</response>
        /// <response code="404">Organização não encontrada</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _deleteOrganizationUseCase.ExecuteAsync(id);
            return NoContent();
        }
    }
}
