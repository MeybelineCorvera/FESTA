using Microsoft.AspNetCore.Mvc;
using FESTA.Data;
using FESTA.Models;
using FESTA.Helpers;

namespace FESTA.Controllers
{
    public class ReservaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string SessionKey = "Carrito";

        public ReservaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var carrito = HttpContext.Session.GetObject<List<ItemCarrito>>(SessionKey);
            if (carrito == null || !carrito.Any())
            {
                TempData["Mensaje"] = "Debe agregar productos antes de hacer una reserva.";
                return RedirectToAction("Index", "Catalogo");
            }

            ViewBag.Total = carrito.Sum(i => i.Producto.Precio * i.Cantidad);
            return View(new Reserva());
        }

        [HttpPost]
        public async Task<IActionResult> Index(Reserva reserva)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Total = 0;
                return View(reserva);
            }

            // Validar anticipación
            var diasAnticipacion = (reserva.FechaEvento - DateTime.Now.Date).TotalDays;

            if (reserva.TipoServicio == "Mobiliario" && diasAnticipacion < 3)
            {
                ModelState.AddModelError("", "Las reservas de mobiliario deben hacerse con al menos 3 días de anticipación.");
                return View(reserva);
            }

            if (reserva.TipoServicio == "Decoración" && diasAnticipacion < 5)
            {
                ModelState.AddModelError("", "Las reservas de decoración deben hacerse con al menos 5 día de anticipación.");
                return View(reserva);
            }

            // ✅ Calcular total desde carrito
            var carrito = HttpContext.Session.GetObject<List<ItemCarrito>>(SessionKey);
            reserva.Total = carrito.Sum(i => i.Producto.Precio * i.Cantidad);

            reserva.FechaRegistro = DateTime.Now;
            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            //Guardar detalles
             foreach (var item in carrito)
            {
                var detalle = new DetalleReserva
                {
                    ReservaId = reserva.Id,
                    ProductoId = item.Producto.Id,
                    Cantidad = item.Cantidad,
                    Subtotal = item.Producto.Precio * item.Cantidad
                };
                _context.DetallesReserva.Add(detalle);
            }
            _context.SaveChanges();
            // Limpiar carrito
            HttpContext.Session.Remove(SessionKey);
            var horaFormateada = DateTime.Today.Add(reserva.HoraEvento).ToString("hh:mm tt");

            // 🔹 Crear mensaje de notificación para WhatsApp
            string mensaje = Uri.EscapeDataString(
                $" Nueva reserva recibida en FESTA " +
                $"*Cliente:* {reserva.NombreCliente}" +
                $" *Fecha del evento:* {reserva.FechaEvento:dd/MM/yyyy}" +
                $" *Hora:* {horaFormateada}%" +
                $" *Dirección:* {reserva.Direccion}" +
                $" *WhatsApp cliente:* {reserva.WhatsApp}" +
                $"*Total estimado:* ${reserva.Total}" +
                $"Por favor confirma la reserva en el sistema "
            );

            // 🔹 Número oficial de FESTA (WhatsApp)
            string whatsappUrl = $"https://wa.me/50379888479?text={mensaje}";

            // Guardar ID para el pago
            TempData["ReservaId"] = reserva.Id;

            // 🔹 Redirigir a página intermedia de confirmación con botón de WhatsApp
            return RedirectToAction("Notificacion", new { whatsapp = whatsappUrl, reservaId = reserva.Id });
        }

        public IActionResult Notificacion(string whatsapp, int reservaId)
        {
            ViewBag.WhatsappUrl = whatsapp;
            ViewBag.ReservaId = reservaId;
            return View();
        }

        public IActionResult Confirmacion()
        {
            return View();
        }
    }
}

