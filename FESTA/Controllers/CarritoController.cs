using Microsoft.AspNetCore.Mvc;
using FESTA.Data;
using FESTA.Models;
using FESTA.Helpers;

namespace FESTA.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string SessionKey = "Carrito";

        public CarritoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            ViewBag.Total = carrito.Sum(i => i.Producto.Precio * i.Cantidad);
            return View(carrito);
        }

        [HttpPost]
        public IActionResult Agregar(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound();

            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.Producto.Id == id);

            if (item != null)
                item.Cantidad++;
            else
                carrito.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });

            GuardarCarrito(carrito);
            return RedirectToAction("Index", "Catalogo");
        }

        public IActionResult Eliminar(int id)
        {
            var carrito = ObtenerCarrito();
            carrito.RemoveAll(i => i.Producto.Id == id);
            GuardarCarrito(carrito);
            return RedirectToAction(nameof(Index));
        }

        private List<ItemCarrito> ObtenerCarrito()
        {
            return HttpContext.Session.GetObject<List<ItemCarrito>>(SessionKey) ?? new List<ItemCarrito>();
        }

        private void GuardarCarrito(List<ItemCarrito> carrito)
        {
            HttpContext.Session.SetObject(SessionKey, carrito);
        }

        [HttpPost]
        public IActionResult ActualizarCantidad(int id, int cantidad)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.Producto.Id == id);

            if (item != null)
            {
                if (cantidad > 0)
                    item.Cantidad = cantidad;
                else
                    carrito.Remove(item); // Si el usuario pone 0, se elimina del carrito
            }

            GuardarCarrito(carrito);
            return RedirectToAction(nameof(Index));
        }

    }
}
