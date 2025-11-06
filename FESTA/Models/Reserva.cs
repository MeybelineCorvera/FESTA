using System;
using System.ComponentModel.DataAnnotations;

namespace FESTA.Models
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar una fecha")]
        [DataType(DataType.Date)]
        public DateTime FechaEvento { get; set; }

        [Required(ErrorMessage = "Debe ingresar una hora")]
        [DataType(DataType.Time)]
        public TimeSpan HoraEvento { get; set; }

        [Required(ErrorMessage = "Debe ingresar una dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Debe ingresar un número de WhatsApp")]
        [Phone]
        public string WhatsApp { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el tipo de servicio")]
        public string TipoServicio { get; set; } = string.Empty;// "Decoración" o "Mobiliario"

        public decimal Total { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Pagado o Cancelado

        // 🔽 NUEVO: productos de la reserva
        
        public List<DetalleReserva>? Detalles { get; set; }
        public List<Pago>? Pagos { get; set; }
    }
}
