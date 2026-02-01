using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Domain.Entities;
using ProjetoFinal.Infrastructure.Persistence;

namespace ProjetoFinal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class TiposDeCursosController : Controller
    {
        private readonly AppDbContext _context;

        public TiposDeCursosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TiposDeCursos
        public async Task<IActionResult> Index()
        {
            var tipos = await _context.TiposCurso.ToListAsync();
            return View(tipos);
        }

        // GET: Admin/TiposDeCursos/Create
        public IActionResult Create()
        {
            return View(); // ← procura Create.cshtml
        }

        // POST: Admin/TiposDeCursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoCurso tipoCurso)
        {
            if (!ModelState.IsValid)
                return View(tipoCurso);

            _context.Add(tipoCurso);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tipo de curso criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/TiposDeCursos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tipoCurso = await _context.TiposCurso.FindAsync(id);
            if (tipoCurso == null) return NotFound();

            return View(tipoCurso); // ← procura Edit.cshtml
        }

        // POST: Admin/TiposDeCursos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoCurso tipoCurso)
        {
            if (id != tipoCurso.Id) return NotFound();
            if (!ModelState.IsValid) return View(tipoCurso);

            try
            {
                _context.Update(tipoCurso);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TiposCurso.Any(t => t.Id == id))
                    return NotFound();
                throw;
            }

            TempData["Success"] = "Tipo de curso atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
