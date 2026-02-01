using ProjetoFinal.Application.DTOs;
namespace ProjetoFinal.Web.Models;

// Filtros vindos da UI (querystring/form) para consultar cursos
public class CursoFilterVm
{
    public int? IdMateria { get; set; }
    public int? IdTipoCurso { get; set; }     // Filtro opcional por tipo de curso
    public string? Q { get; set; }             // Busca textual (nome/descrição)
    public string? SortBy { get; set; } = "CreatedAt"; // Campo de ordenação padrão
    public bool Desc { get; set; } = true;     // Ordem decrescente por padrão
    public int Page { get; set; } = 1;         // Página atual
    public int PageSize { get; set; } = 12;    // Itens por página
}

// ViewModel da página de cursos: agrega filtros, dados paginados e listas auxiliares para selects
public class CursoPageVm
{
    public CursoFilterVm Filter { get; set; } = new();                // Filtros aplicados
    public PagedResult<CursoDto>? PageResult { get; set; }            // Resultado paginado (DTOs)
    public IReadOnlyList<(int Id, string Nome)> Materias { get; set; } = Array.Empty<(int, string)>();   // Lista para <select>
    public IReadOnlyList<(int Id, string Nome)> TiposCurso { get; set; } = Array.Empty<(int, string)>(); // Lista para <select>

    public IReadOnlyList<CursoDto> Destaques { get; set; } = Array.Empty<CursoDto>();
}
