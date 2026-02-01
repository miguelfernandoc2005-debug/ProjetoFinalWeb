using ProjetoFinal.Domain.Entities;

namespace ProjetoFinal.Application.Abstractions.Repositories;

public interface ITipoCursoRepository
{
    Task<IReadOnlyList<TipoCurso>> GetAllAsync(CancellationToken ct = default);
}
