using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjetoFinal.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "ManagerOrAdmin")]
public class ManageController : Controller
{
    [HttpGet]
    public IActionResult Index()
        => View();

    // Placeholders para os CRUDs
    [HttpGet]
    public IActionResult Cursos()
        => View();

    [HttpGet]
    public IActionResult Materias()
        => View();

    [HttpGet]
    public IActionResult TiposCurso()
        => View();

    [HttpGet]
    public IActionResult Usuarios()
        => View();
}
