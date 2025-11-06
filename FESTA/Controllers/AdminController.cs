using Microsoft.AspNetCore.Mvc;
using FESTA.Data;
using FESTA.Models;
using Microsoft.EntityFrameworkCore;



namespace FESTA.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Página de login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var admin = _context.Administradores.FirstOrDefault(a => a.Email == email && a.Password == password);
            if (admin != null)
            {
                HttpContext.Session.SetString("Admin", admin.Email);
                return RedirectToAction("Panel", "Admin");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        // Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Login");
        }

        // Panel de administración
        public IActionResult Panel()
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login");

            ViewBag.TotalProductos = _context.Productos.Count();
            ViewBag.TotalReservas = _context.Reservas.Count();
            ViewBag.TotalPagos = _context.Pagos.Count();
            ViewBag.AdminActual = HttpContext.Session.GetString("Admin");

            var productos = _context.Productos.Take(100).ToList();

            // --- NUEVO: Datos para gráficos ---
            // Agrupamos reservas y pagos por mes (últimos 6 meses)
            var meses = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .OrderBy(m => m)
                .ToList();

            var reservasPorMes = meses.Select(m =>
                _context.Reservas.Count(r => r.FechaEvento.Month == m.Month && r.FechaEvento.Year == m.Year)
                ).ToList();

            var pagosPorMes = meses.Select(m =>
                _context.Pagos.Count(p => p.FechaPago.Month == m.Month && p.FechaPago.Year == m.Year)
            ).ToList();

            ViewBag.Meses = meses.Select(m => m.ToString("MMM yyyy")).ToList();
            ViewBag.ReservasPorMes = reservasPorMes;
            ViewBag.PagosPorMes = pagosPorMes;

            return View(productos);
        }

        // 📋 Ver todas las reservas
        public async Task<IActionResult> Reservas()
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Admin");

            var reservas = await _context.Reservas
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(r => r.Pagos) // 👈 NUEVO: incluir pagos
                .OrderByDescending(r => r.FechaRegistro)
                .ToListAsync();

            return View(reservas);
        }

        // ✅ Cambiar el estado de una reserva
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();

            reserva.Estado = nuevoEstado;
            _context.Update(reserva);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"La reserva #{id} ha sido marcada como {nuevoEstado}.";
            return RedirectToAction(nameof(Reservas));
        }
    }
}
