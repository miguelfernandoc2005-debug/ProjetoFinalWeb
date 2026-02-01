using ProjetoFinal.Application.Abstractions.Repositories;
using ProjetoFinal.Application.DTOs;
using ProjetoFinal.Application.Filters;
using ProjetoFinal.Application.Services;

namespace ProjetoFinal.Infrastructure.Services;
public sealed class CursoQueryService : ICursoQueryService
{
    private readonly ICursoRepository _repo;

    public CursoQueryService(ICursoRepository repo) => _repo = repo;

    public async Task<PagedResult<CursoDto>> SearchAsync(CursoFilter filter, CancellationToken ct = default)
    {
        var (items, total) = await _repo.SearchAsync(filter, ct);

        var dtos = items.Select(c => new CursoDto(
            c.Id,
            c.Nome,
            c.Descricao,
            c.ImagemCapaUrl,
            c.CargaHoraria,
            c.DataDeCriacao,
            c.IdMateria,
            c.Materia?.Nome,
            c.IdTipoCurso,
            c.TipoCurso?.Nome
        )).ToList();

        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = filter.PageSize < 1 ? 12 : filter.PageSize;
        if (pageSize > 48) pageSize = 48;

        return new PagedResult<CursoDto>(page, pageSize, total, dtos);
    }
}
