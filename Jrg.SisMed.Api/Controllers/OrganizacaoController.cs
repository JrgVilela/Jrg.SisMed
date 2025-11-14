using Jrg.SisMed.Application.DTOs.OrganizationDto;
using Jrg.SisMed.Application.UseCases.Organization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web;

namespace Jrg.SisMed.Api.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _readOrganizationUseCase.GetAllAsync(cancellationToken);

            if(!result.Any())
                return NotFound("No organizations found.");

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var result = await _readOrganizationUseCase.GetByIdAsync(id, cancellationToken);

            if (result == null)
                return NotFound($"Organization with id {id} not found.");

            return Ok(result);
        }

        /// <summary>
        /// Busca organização por CNPJ usando query string (alternativa mais limpa).
        /// Exemplo: GET /api/organizacao/search?cnpj=43.133.410/0001-13
        /// </summary>
        /// <param name="cnpj">CNPJ com ou sem formatação</param>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string cnpj, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return BadRequest("CNPJ is required");

            var result = await _readOrganizationUseCase.GetByCnpjAsync(cnpj, cancellationToken);

            if (result == null)
                return NotFound($"Organization with CNPJ {cnpj} not found.");

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateOrganizationDto createOrganizationDto, CancellationToken cancellationToken)
        {
            var result = await _createOrganizationUseCase.ExecuteAsync(createOrganizationDto, cancellationToken);

            if(result == null || result <= 0)
                return BadRequest("Failed to create organization.");

            return CreatedAtAction(nameof(Get), new { id = result }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateOrganizationDto updateOrganizationDto, CancellationToken cancellationToken)
        {
            await _updateOrganizationUseCase.ExecuteAsync(id, updateOrganizationDto, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _deleteOrganizationUseCase.ExecuteAsync(id);
            return NoContent();
        }
    }
}
