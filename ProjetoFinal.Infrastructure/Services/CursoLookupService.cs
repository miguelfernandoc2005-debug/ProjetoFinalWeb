using ProjetoFinal.Application.Abstractions.Repositories;
using ProjetoFinal.Application.Services;

namespace ProjetoFinal.Infrastructure.Services;

public sealed class CursoLookupService : ICursoLookupService
{
    private readonly IMateriaRepository _materias;
    private readonly ITipoCursoRepository _tipos;

    public CursoLookupService(IMateriaRepository materias, ITipoCursoRepository tipos)
    {
        _materias = materias;
        _tipos = tipos;
    }

    public async Task<IReadOnlyList<(int Id, string Nome)>> GetMateriasAsync(CancellationToken ct = default)
    {
        var list = await _materias.GetAllAsync(ct);
        return list.Select(m => (m.Id, m.Nome)).ToList();
    }

    public async Task<IReadOnlyList<(int Id, string Nome)>> GetTiposCursoAsync(CancellationToken ct = default)
    {
        var list = await _tipos.GetAllAsync(ct);
        return list.Select(t => (t.Id, t.Nome)).ToList();
    }
}
