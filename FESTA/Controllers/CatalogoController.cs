using FESTA.Data;
using FESTA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FESTA.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Subcategorias)
                .Where(c => c.CategoriaPadreId == null)
                .ToListAsync();

            return View(categorias);
        }


        public async Task<IActionResult> Detalle(int id)
        {
            var producto = await _context.Productos.Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // CRUD básico para administración
        public IActionResult Crear()
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            // Cargar categorías para el dropdown
            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto)
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
                return View(producto);
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // ✅ Guardar cambios en edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Producto producto)
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
                return View(producto);
            }

            try
            {
                _context.Update(producto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewBag.Error = "⚠️ Error: asegúrate de seleccionar una categoría válida.";
                ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
                return View(producto);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Eliminar(int id)
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> VerCategoria(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Subcategorias)
                    .ThenInclude(s => s.Productos)
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

    }
}
