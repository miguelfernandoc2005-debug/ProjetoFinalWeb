using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoFinal.Web.Areas.Admin.Services;

namespace ProjetoFinal.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class DashboardController : Controller
{
    private readonly DashboardService _svc;

    public DashboardController(DashboardService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var totalCursos = await _svc.TotalCursosAsync(ct);
        var totalMaterias = await _svc.TotalMateriasAsync(ct);
        var totalUsuarios = await _svc.TotalUsuariosAsync(ct);
        var porTipoUsuario = await _svc.TotalUsuariosPorTipoAsync(ct);
        var ultimos = await _svc.UltimosCursosAsync(5, ct);

        ViewBag.TotalCursos = totalCursos;
        ViewBag.TotalMaterias = totalMaterias;
        ViewBag.TotalUsuarios = totalUsuarios;
        ViewBag.PorTipoUsuario = porTipoUsuario;
        ViewBag.Ultimos = ultimos;

        return View("~/Areas/Admin/Views/Dashboard/Index.cshtml");
    }

    // Endpoint de dados para os gráficos (JSON)
    [HttpGet("Admin/Dashboard/Data")]
    public async Task<IActionResult> Data(CancellationToken ct)
    {
        var cursosPorMateria = await _svc.CursosPorMateriaAsync(ct);
        var cursosPorTipo = await _svc.CursosPorTipoAsync(ct);
        var cursosPorAno = await _svc.CursosPorAnoAsync(ct);

        return Json(new
        {
            cursosPorMateria = new
            {
                labels = cursosPorMateria.Select(x => x.Label).ToArray(),
                data = cursosPorMateria.Select(x => x.Qtd).ToArray()
            },
            cursosPorTipo = new
            {
                labels = cursosPorTipo.Select(x => x.Label).ToArray(),
                data = cursosPorTipo.Select(x => x.Qtd).ToArray()
            },
            cursosPorAno = new
            {
                labels = cursosPorAno.Select(x => x.Ano).ToArray(),
                data = cursosPorAno.Select(x => x.Qtd).ToArray()
            }
        });
    }
}
