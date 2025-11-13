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
        public async Task<IActionResult> Crear(Categoria categoria, List<IFormFile> imagenes)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoriasPadre = _context.Categorias.ToList();
                return View(categoria);
            }
            if (categoria.ImagenArchivo != null)
            {
                string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categorias");
                Directory.CreateDirectory(carpeta);

                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(categoria.ImagenArchivo.FileName);
                string ruta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await categoria.ImagenArchivo.CopyToAsync(stream);
                }

                categoria.ImagenUrl = "/images/categorias/" + nombreArchivo;
            }

            _context.Add(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }

        // ✏️ Editar categoría
        public async Task<IActionResult> Editar(int id)
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();

            ViewBag.CategoriasPadre = _context.Categorias
                .Where(c => c.Id != id && c.CategoriaPadreId == null)
                .ToList();

            return View("Crear", categoria); // 👈 reutiliza la misma vista de creación
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoria, IFormFile? ImagenArchivo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoriasPadre = _context.Categorias.ToList();
                return View(categoria);
            }

            var categoriaDb = await _context.Categorias.FindAsync(categoria.Id);
            if (categoriaDb == null)
                return NotFound();

            categoriaDb.Nombre = categoria.Nombre;
            categoriaDb.Descripcion = categoria.Descripcion;
            categoriaDb.CategoriaPadreId = categoria.CategoriaPadreId;

            if (ImagenArchivo != null)
            {
                string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categorias");
                Directory.CreateDirectory(carpeta);
                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(ImagenArchivo.FileName);
                string ruta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await ImagenArchivo.CopyToAsync(stream);
                }

                categoriaDb.ImagenUrl = "/images/categorias/" + nombreArchivo;
            }

            _context.Update(categoriaDb);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

        // ➕ Crear subcategoría (desde modal)
        [HttpPost]
        public async Task<IActionResult> CrearSubcategoria(string nombre, string descripcion, int categoriaPadreId, IFormFile? imagenArchivo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                TempData["ErrorMensaje"] = "⚠️ El nombre de la subcategoría es obligatorio.";
                return RedirectToAction(nameof(Index));
            }

            var subcategoria = new Categoria
            {
                Nombre = nombre,
                Descripcion = descripcion,
                CategoriaPadreId = categoriaPadreId
            };

            if (imagenArchivo != null)
            {
                string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categorias");
                Directory.CreateDirectory(carpeta);

                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagenArchivo.FileName);
                string ruta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await imagenArchivo.CopyToAsync(stream);
                }

                subcategoria.ImagenUrl = "/images/categorias/" + nombreArchivo;
            }

            _context.Categorias.Add(subcategoria);
            await _context.SaveChangesAsync();

            TempData["ExitoMensaje"] = "✅ Subcategoría agregada correctamente.";
            return RedirectToAction(nameof(Index));
        }


    }
}
