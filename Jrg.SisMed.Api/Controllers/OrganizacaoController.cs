using Jrg.SisMed.Application.UseCases.Organization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [HttpGet("cnpj/{cnpj}")]
        public async Task<IActionResult> Get(string cnpj)
        {
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
