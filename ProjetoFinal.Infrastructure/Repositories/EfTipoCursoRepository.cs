using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Application.Abstractions.Repositories;
using ProjetoFinal.Domain.Entities;
using ProjetoFinal.Infrastructure.Persistence;

namespace ProjetoFinal.Infrastructure.Repositories
{
    public sealed class EfTipoCursoRepository : ITipoCursoRepository
    {
        private readonly AppDbContext _ctx;

        public EfTipoCursoRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<IReadOnlyList<TipoCurso>> GetAllAsync(CancellationToken ct = default) =>
            await _ctx.TiposCurso
                .AsNoTracking()
                .OrderBy(t => t.Nome)
                .ToListAsync(ct);
    }
}
