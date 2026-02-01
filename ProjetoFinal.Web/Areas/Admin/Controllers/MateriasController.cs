using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Infrastructure.Persistence;
using ProjetoFinal.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoFinal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class MateriasController : Controller
    {
        private readonly AppDbContext _context;

        public MateriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Materias
        public async Task<IActionResult> Index()
        {
            var materias = await _context.Materias
                .Include(m => m.Cursos)
                .ToListAsync();

            return View(materias);
        }

        // GET: Admin/Materias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var materia = await _context.Materias
                .Include(m => m.Cursos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (materia == null) return NotFound();

            return View(materia);
        }

        // GET: Admin/Materias/Create
        public IActionResult Create() => View();

        // POST: Admin/Materias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Materia materia)
        {
            if (!ModelState.IsValid) return View(materia);

            _context.Add(materia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Materias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var materia = await _context.Materias
                .Include(m => m.Cursos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (materia == null) return NotFound();

            return View(materia);
        }

        // POST: Admin/Materias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Materia materia)
        {
            if (id != materia.Id) return NotFound();
            if (!ModelState.IsValid) return View(materia);

            _context.Update(materia);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Materias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var materia = await _context.Materias.FindAsync(id);
            if (materia == null) return NotFound();

            return View(materia);
        }

        // POST: Admin/Materias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materia = await _context.Materias.FindAsync(id);
            if (materia != null)
            {
                _context.Materias.Remove(materia);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
