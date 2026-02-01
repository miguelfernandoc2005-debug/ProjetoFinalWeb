using ProjetoFinal.Domain.Entities;

namespace ProjetoFinal.Application.Abstractions.Repositories;

public interface IMateriaRepository
{
    Task<IReadOnlyList<Materia>> GetAllAsync(CancellationToken ct = default);
}
