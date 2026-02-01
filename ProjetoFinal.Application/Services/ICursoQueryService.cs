using ProjetoFinal.Application.DTOs;
using ProjetoFinal.Application.Filters;

namespace ProjetoFinal.Application.Services;

/// <summary>
/// Serviço de aplicação para consultar o catálogo de cursos com filtros e paginação.
/// </summary>
public interface ICursoQueryService
{
    /// <summary>
    /// Pesquisa cursos aplicando filtros, ordenação e paginação, e retorna o resultado paginado.
    /// </summary>
    /// <param name="filter">Critérios de filtro/paginação/ordenação (ex.: Matéria, TipoCurso, busca textual, SortBy, Desc, Page, PageSize).</param>
    /// <param name="ct">Token de cancelamento para operações assíncronas.</param>
    /// <returns>
    /// Um <see cref="PagedResult{T}"/> de <see cref="CursoDto"/>, contendo a página atual de cursos e metadados (ex.: Total).
    /// </returns>
    Task<PagedResult<CursoDto>> SearchAsync(CursoFilter filter, CancellationToken ct = default);
}
