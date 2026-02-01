namespace ProjetoFinal.Application.DTOs;

/// <summary>
/// DTO enxuto para listagem de cursos no catálogo.
/// </summary>
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
