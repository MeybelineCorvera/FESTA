using Microsoft.AspNetCore.Mvc;
using FESTA.Data;
using FESTA.Models;
using Microsoft.EntityFrameworkCore;

namespace FESTA.Controllers
{
    public class PagoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PagoController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Mostrar formulario de pago
        public IActionResult Index(int reservaId)
        {
            var reserva = _context.Reservas.FirstOrDefault(r => r.Id == reservaId);
            if (reserva == null) return NotFound();

            ViewBag.Reserva = reserva;
            return View(new Pago { ReservaId = reservaId });
        }

        [HttpPost]
        public async Task<IActionResult> Index(Pago pago, IFormFile? Comprobante)
        {
            var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.Id == pago.ReservaId);
            if (reserva == null) return NotFound();

            // Calcular monto según porcentaje
            pago.MontoPagado = reserva.Total * (pago.PorcentajePago / 100m);

            // Guardar comprobante
            if (Comprobante != null && Comprobante.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "comprobantes");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}_{Comprobante.FileName}";
                var filePath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await Comprobante.CopyToAsync(stream);
                pago.ComprobanteUrl = "/comprobantes/" + fileName;
            }

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            TempData["PagoConfirmado"] = pago.Id;
            return RedirectToAction("Confirmacion", new { id = pago.Id });
        }

        public IActionResult Confirmacion(int id)
        {
            var pago = _context.Pagos
                .Include(p => p.Reserva)
                .FirstOrDefault(p => p.Id == id);

            if (pago == null) return NotFound();
            return View(pago);
        }
    }
}
