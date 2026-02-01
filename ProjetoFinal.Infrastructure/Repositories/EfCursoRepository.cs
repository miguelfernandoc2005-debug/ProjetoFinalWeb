using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Application.Abstractions.Repositories;
using ProjetoFinal.Application.Filters;
using ProjetoFinal.Domain.Entities;
using ProjetoFinal.Infrastructure.Persistence;

namespace ProjetoFinal.Infrastructure.Repositories
{
    public sealed class EfCursoRepository : ICursoRepository
    {
        private readonly AppDbContext _ctx;

        public EfCursoRepository(AppDbContext ctx) => _ctx = ctx;

        // Pesquisa cursos com filtros, ordenação e paginação
        public async Task<(IReadOnlyList<Curso> Items, int Total)> SearchAsync(
            CursoFilter filter, CancellationToken ct = default)
        {
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1 ? 12 : filter.PageSize;
            if (pageSize > 48) pageSize = 48;

            var q = _ctx.Cursos
                .AsNoTracking()
                .Include(c => c.Materia)
                .Include(c => c.TipoCurso)
                .AsQueryable();

            if (filter.IdMateria.HasValue)
                q = q.Where(c => c.IdMateria == filter.IdMateria.Value);

            if (filter.IdTipoCurso.HasValue)
                q = q.Where(c => c.IdTipoCurso == filter.IdTipoCurso.Value);

            if (!string.IsNullOrWhiteSpace(filter.Q))
            {
                var like = $"%{filter.Q.Trim()}%";
                q = q.Where(c =>
                    EF.Functions.Like(c.Nome, like) ||
                    (c.Descricao != null && EF.Functions.Like(c.Descricao, like)));
            }

            q = (filter.SortBy?.ToLowerInvariant()) switch
            {
                "nome" => filter.Desc ? q.OrderByDescending(c => c.Nome) : q.OrderBy(c => c.Nome),
                "cargahoraria" => filter.Desc ? q.OrderByDescending(c => c.CargaHoraria) : q.OrderBy(c => c.CargaHoraria),
                "datadecriacao" => filter.Desc ? q.OrderByDescending(c => c.DataDeCriacao) : q.OrderBy(c => c.DataDeCriacao),
                _ => q.OrderByDescending(c => c.DataDeCriacao)
            };

            var total = await q.CountAsync(ct);

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public Task<Curso?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _ctx.Cursos
                .AsNoTracking()
                .Include(c => c.Materia)
                .Include(c => c.TipoCurso)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}
