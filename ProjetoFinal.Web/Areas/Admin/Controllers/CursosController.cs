using Microsoft.AspNetCore.Mvc;
using ProjetoFinal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoFinal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class CursosController : Controller
    {
        private readonly AppDbContext _context;

        public CursosController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 LISTAGEM
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos
                .Include(c => c.Materia)
                .Include(c => c.TipoCurso)
                .ToListAsync();

            return View(cursos);
        }

        // 🔹 DETALHES
        public async Task<IActionResult> Details(int id)
        {
            var curso = await _context.Cursos
                .Include(c => c.Materia)
                .Include(c => c.TipoCurso)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();
            return View(curso);
        }

        // 🔹 CRIAÇÃO
        public IActionResult Create()
        {
            ViewBag.Materias = new SelectList(_context.Materias, "Id", "Nome");
            ViewBag.TiposCurso = new SelectList(_context.TiposCurso, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Materias = _context.Materias.ToList();
                ViewBag.TiposCurso = _context.TiposCurso.ToList();
                return View(curso);
            }

            // Aqui ImagemCapaUrl já vem preenchida da view (campo URL)
            _context.Add(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔹 EDIÇÃO
        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            ViewBag.Materias = new SelectList(_context.Materias, "Id", "Nome", curso.IdMateria);
            ViewBag.TiposCurso = new SelectList(_context.TiposCurso, "Id", "Nome", curso.IdTipoCurso);
            return View(curso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Materias = _context.Materias.ToList();
                ViewBag.TiposCurso = _context.TiposCurso.ToList();
                return View(curso);
            }

            // ImagemCapaUrl também já vem preenchida da view
            _context.Update(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔹 EXCLUSÃO
        public async Task<IActionResult> Delete(int id)
        {
            var curso = await _context.Cursos
                .Include(c => c.Materia)
                .Include(c => c.TipoCurso)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();
            return View(curso);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            _context.Cursos.Remove(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
