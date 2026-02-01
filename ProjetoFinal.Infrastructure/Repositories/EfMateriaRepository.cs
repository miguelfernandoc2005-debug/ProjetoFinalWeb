using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Application.Abstractions.Repositories;
using ProjetoFinal.Domain.Entities;
using ProjetoFinal.Infrastructure.Persistence;

namespace ProjetoFinal.Infrastructure.Repositories
{
    public sealed class EfMateriaRepository : IMateriaRepository
    {
        private readonly AppDbContext _ctx;

        public EfMateriaRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<IReadOnlyList<Materia>> GetAllAsync(CancellationToken ct = default) =>
            await _ctx.Materias
                .AsNoTracking()
                .OrderBy(m => m.Nome)
                .ToListAsync(ct);
    }
}
