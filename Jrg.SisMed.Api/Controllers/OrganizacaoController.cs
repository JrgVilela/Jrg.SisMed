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
        public async Task<IActionResult> Get()
        {
            var result = await _readOrganizationUseCase.GetAllAsync();

            if(!result.Any())
                return NotFound("No organizations found.");

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _readOrganizationUseCase.GetByIdAsync(id);

            if (result == null)
                return NotFound($"Organization with id {id} not found.");

            return Ok(result);
        }

        /// <summary>
        /// Busca organização por CNPJ (apenas números ou formatado).
        /// </summary>
        /// <param name="cnpj">CNPJ com ou sem formatação (ex: 43133410000113 ou 43.133.410/0001-13)</param>
        //[HttpGet("cnpj/{cnpj}")]
        //public async Task<IActionResult> GetByCnpj(string cnpj)
        //{
        //    // Decodifica o CNPJ (converte %2F de volta para /)
        //    var decodedCnpj = Uri.UnescapeDataString(cnpj);
            
        //    var result = await _readOrganizationUseCase.GetByCnpjAsync(decodedCnpj);

        //    if (result == null)
        //        return NotFound($"Organization with CNPJ {decodedCnpj} not found.");

        //    return Ok(result);
        //}

        /// <summary>
        /// Busca organização por CNPJ usando query string (alternativa mais limpa).
        /// Exemplo: GET /api/organizacao/search?cnpj=43.133.410/0001-13
        /// </summary>
        /// <param name="cnpj">CNPJ com ou sem formatação</param>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return BadRequest("CNPJ is required");

            var result = await _readOrganizationUseCase.GetByCnpjAsync(cnpj);

            if (result == null)
                return NotFound($"Organization with CNPJ {cnpj} not found.");

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post()
        {
            return Ok("OrganizacaoController POST is working!");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id)
        {
            return Ok($"OrganizacaoController PUT is working with id: {id}");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"OrganizacaoController DELETE is working with id: {id}");
        }
    }
}
