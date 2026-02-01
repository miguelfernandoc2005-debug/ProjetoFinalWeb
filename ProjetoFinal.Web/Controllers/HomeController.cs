using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Web.Models;          // DTOs/ViewModels para Cursos
using ProjetoFinal.Web.Services;        // IApiClient para consumir API de Cursos

namespace ProjetoFinal.Web.Controllers;

public class HomeController(IApiClient api, ILogger<HomeController> log) : Controller
{
    private readonly IApiClient _api = api;
    private readonly ILogger<HomeController> _log = log;

    // Monta o ViewModel da página de cursos, normalizando paginação e preenchendo listas auxiliares
    private async Task<CursoPageVm> BuildVm(CursoFilterVm filter, CancellationToken ct)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1 || filter.PageSize > 48) filter.PageSize = 12;

        return new CursoPageVm
        {
            Filter = filter,
            Materias = await _api.GetMateriasAsync(ct),
            TiposCurso = await _api.GetTiposCursoAsync(ct),
            PageResult = await _api.SearchCursosAsync(filter, ct)
        };
    }


    // Página principal: responde a "/" e "/Home/Index"
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] CursoFilterVm filter, string? section, CancellationToken ct)
    {
        var vm = await BuildVm(filter, ct);

        // ? Aqui você chama o mesmo método, mas sem filtro
        vm.Destaques = (await _api.SearchCursosAsync(
            new CursoFilterVm { Page = 1, PageSize = 9999 }, ct
        )).Items;

        ViewBag.Section = section;
        return View(vm);
    }


    // Compatibilidade: /Home/Cursos -> Index em #cursos
    [HttpGet]
    public Task<IActionResult> Cursos([FromQuery] CursoFilterVm filter, CancellationToken ct)
        => Index(filter, "cursos", ct);

    // Compatibilidade: /Home/Materias -> Index em #materias
    [HttpGet]
    public Task<IActionResult> Materias(CancellationToken ct)
        => Index(new CursoFilterVm(), "materias", ct);

    // Compatibilidade: /Home/TiposCurso -> Index em #tiposCurso
    [HttpGet]
    public Task<IActionResult> TiposCurso(CancellationToken ct)
        => Index(new CursoFilterVm(), "tiposCurso", ct);
}