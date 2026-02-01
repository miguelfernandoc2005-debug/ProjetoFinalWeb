using Microsoft.AspNetCore.Mvc;
using ProjetoFinal.Application.Services;
using ProjetoFinal.Infrastructure.Persistence;
using ProjetoFinal.Domain;// ICursoLookupService (ou IMateriaLookupService, se preferir separar)

namespace ProjetoFinal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => api/materias
    public class MateriasController : ControllerBase
    {
        private readonly ICursoLookupService _lookup;

        public MateriasController(ICursoLookupService lookup)
        {
            _lookup = lookup;
        }

        [HttpGet] // GET api/materias
        public async Task<ActionResult<object>> Get(CancellationToken ct)
        {
            var items = await _lookup.GetMateriasAsync(ct); // Retorna tuplas (Id, Nome)
            return Ok(items.Select(x => new { x.Id, x.Nome })); // Shape explícito
        }
    }
}
