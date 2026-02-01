using Microsoft.AspNetCore.Mvc;
using ProjetoFinal.Application.DTOs;
using ProjetoFinal.Application.Filters;
using ProjetoFinal.Application.Services;
using ProjetoFinal.Application.Abstractions.Repositories;

namespace ProjetoFinal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => api/cursos
    public class CursosController : ControllerBase
    {
        private readonly ICursoQueryService _service;
        private readonly ICursoRepository _repo;

        public CursosController(ICursoQueryService service, ICursoRepository repo)
        {
            _service = service;
            _repo = repo;
        }

        /// <summary>Consulta paginada com filtros/ordenação.</summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<CursoDto>>> Get([FromQuery] CursoFilter filter, CancellationToken ct)
        {
            var result = await _service.SearchAsync(filter, ct);

            // Cabeçalho para total de itens (usado pelo front-end)
            Response.Headers["X-Total-Count"] = result.Total.ToString();

            return Ok(result);
        }

        /// <summary>Consulta por ID.</summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CursoDto>> GetById(int id, CancellationToken ct)
        {
            var c = await _repo.GetByIdAsync(id, ct);
            if (c is null) return NotFound();

            var dto = new CursoDto(
                c.Id,
                c.Nome,
                c.Descricao,
                c.ImagemCapaUrl,
                c.CargaHoraria,
                c.DataDeCriacao,
                c.IdMateria,
                c.Materia?.Nome ?? string.Empty,
                c.IdTipoCurso,
                c.TipoCurso?.Nome ?? string.Empty
            );

            return Ok(dto);
        }
    }
}