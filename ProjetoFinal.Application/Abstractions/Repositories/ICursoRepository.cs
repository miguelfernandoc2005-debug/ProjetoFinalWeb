using ProjetoFinal.Application.Filters;
using ProjetoFinal.Domain.Entities;

namespace ProjetoFinal.Application.Abstractions.Repositories;

public interface ICursoRepository
{
    Task<(IReadOnlyList<Curso> Items, int Total)> SearchAsync(
        CursoFilter filter,
        CancellationToken ct = default);

    Task<Curso?> GetByIdAsync(int id, CancellationToken ct = default);
}
