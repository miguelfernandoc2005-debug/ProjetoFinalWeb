using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Infrastructure.Persistence;
using ProjetoFinal.Domain.Entities;

namespace ProjetoFinal.Web.Areas.Admin.Services;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    // ---------- Cards ----------
    public async Task<int> TotalCursosAsync(CancellationToken ct = default)
        => await _db.Cursos.AsNoTracking().CountAsync(ct);

    public async Task<int> TotalMateriasAsync(CancellationToken ct = default)
        => await _db.Materias.AsNoTracking().CountAsync(ct);

    public async Task<int> TotalUsuariosAsync(CancellationToken ct = default)
        => await _db.TiposUsuario.AsNoTracking().CountAsync(ct);

    public async Task<Dictionary<string, int>> TotalUsuariosPorTipoAsync(CancellationToken ct = default)
    {
        // Conta usuários por tipo (ex.: Administrador, Docente, Aluno)
        return await _db.TiposUsuario
                        .AsNoTracking()
                        .GroupBy(t => t.Nome)
                        .Select(g => new { Tipo = g.Key, Qtd = g.Count() })
                        .ToDictionaryAsync(x => x.Tipo, x => x.Qtd, ct);
    }

    // ---------- Gráficos ----------
    public async Task<List<(string Label, int Qtd)>> CursosPorMateriaAsync(CancellationToken ct = default)
    {
        var query = from c in _db.Cursos
                    join m in _db.Materias on c.IdMateria equals m.Id
                    group c by m.Nome into grp
                    orderby grp.Key
                    select new { Label = grp.Key, Qtd = grp.Count() };

        var data = await query.AsNoTracking().ToListAsync(ct);
        return data.Select(x => (x.Label, x.Qtd)).ToList();
    }

    public async Task<List<(string Label, int Qtd)>> CursosPorTipoAsync(CancellationToken ct = default)
    {
        var query = from c in _db.Cursos
                    join t in _db.TiposCurso on c.IdTipoCurso equals t.Id
                    group c by t.Nome into grp
                    orderby grp.Key
                    select new { Label = grp.Key, Qtd = grp.Count() };

        var data = await query.AsNoTracking().ToListAsync(ct);
        return data.Select(x => (x.Label, x.Qtd)).ToList();
    }

    public async Task<List<(int Ano, int Qtd)>> CursosPorAnoAsync(CancellationToken ct = default)
    {
        var data = await _db.Cursos
            .AsNoTracking()
            .GroupBy(c => c.DataDeCriacao.Year)
            .Select(g => new { Ano = g.Key, Qtd = g.Count() })
            .OrderBy(x => x.Ano)
            .ToListAsync(ct);

        return data.Select(x => (x.Ano, x.Qtd)).ToList();
    }

    // ---------- Tabela auxiliar ----------
    public async Task<List<UltimoCursoItem>> UltimosCursosAsync(int take = 5, CancellationToken ct = default)
    {
        var q = from c in _db.Cursos
                join m in _db.Materias on c.IdMateria equals m.Id
                join t in _db.TiposCurso on c.IdTipoCurso equals t.Id
                orderby c.DataDeCriacao descending
                select new UltimoCursoItem
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    DataDeCriacao = c.DataDeCriacao,
                    Materia = m.Nome,
                    TipoCurso = t.Nome,
                    CargaHoraria = c.CargaHoraria
                };

        return await q.AsNoTracking().Take(take).ToListAsync(ct);
    }
}

public class UltimoCursoItem
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public DateTime DataDeCriacao { get; set; }
    public string Materia { get; set; } = "";
    public string TipoCurso { get; set; } = "";
    public int CargaHoraria { get; set; }
}
