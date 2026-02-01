namespace ProjetoFinal.Web.Models;

// DTO genérico para paginação (espelha o PagedResult<T> da API)
public record PagedResult<T>(
    int Page,            // Página atual (1-based)
    int PageSize,        // Quantidade de itens por página
    int Total,           // Total de registros encontrados
    IReadOnlyList<T> Items
);

// DTO do Curso (espelha a resposta da API)
public record CursoDto(
    int Id,
    string Nome,
    string? Descricao,
    string? ImagemCapaUrl,
    int CargaHoraria,
    DateTime DataDeCriacao,
    int? IdMateria,
    string? NomeMateria,
    int? IdTipoCurso,
    string? NomeTipoCurso
);
