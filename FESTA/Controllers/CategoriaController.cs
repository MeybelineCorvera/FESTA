using Microsoft.AspNetCore.Mvc;
using FESTA.Data;
using FESTA.Models;
using Microsoft.EntityFrameworkCore;

namespace FESTA.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📋 Listado de categorías
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            var categorias = await _context.Categorias
                .Include(c => c.Subcategorias)
                .Where(c => c.CategoriaPadreId == null)
                .ToListAsync();
            return View(categorias);
        }

        // ➕ Crear categoría
        public IActionResult Crear()
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");
            ViewBag.CategoriasPadre = _context.Categorias
                .Where(c => c.CategoriaPadreId == null)
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoriasPadre = _context.Categorias.ToList();
                return View(categoria);
            }

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ✏️ Editar categoría
        public async Task<IActionResult> Editar(int id)
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            var categoria = await _context.Categorias.FindAsync(id);
            ViewBag.CategoriasPadre = _context.Categorias
                .Where(c => c.Id != id && c.CategoriaPadreId == null)
                .ToList();
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoriasPadre = _context.Categorias.ToList();
            return View(categoria);
        }

        // 🗑️ Eliminar categoría
        // 🗑️ Eliminar categoría con validación
        public async Task<IActionResult> Eliminar(int id)
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            var categoria = await _context.Categorias
                .Include(c => c.Subcategorias)
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            // ⚠️ Verificar si tiene subcategorías o productos
            if ((categoria.Subcategorias != null && categoria.Subcategorias.Any()) ||
                (categoria.Productos != null && categoria.Productos.Any()))
            {
                TempData["ErrorMensaje"] = "⚠️ No puedes eliminar esta categoría porque tiene subcategorías o productos asociados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            TempData["ExitoMensaje"] = "✅ Categoría eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
