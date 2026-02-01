using Microsoft.AspNetCore.Mvc;
using ProjetoFinal.Application.Services; // ICursosLookupService ou ICursoLookupService

namespace ProjetoFinal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoDeCursosController : ControllerBase
    {
        private readonly ICursoLookupService _lookup;

        public TipoDeCursosController(ICursoLookupService lookup) => _lookup = lookup;

        [HttpGet] // GET api/tipodecursos
        public async Task<ActionResult<object>> Get(CancellationToken ct)
        {
            var items = await _lookup.GetTiposCursoAsync(ct); // Método correto da interface
            return Ok(items.Select(x => new { x.Id, x.Nome })); // Shape explícito para a UI
        }
    }
}
