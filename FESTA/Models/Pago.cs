using System;
using System.ComponentModel.DataAnnotations;

namespace FESTA.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el método de pago")]
        public string MetodoPago { get; set; }  // "Transferencia bancaria"

        [Required(ErrorMessage = "Debe seleccionar porcentaje de pago")]
        public int PorcentajePago { get; set; } // 50 o 100

        [Required]
        public decimal MontoPagado { get; set; }

        public string? ComprobanteUrl { get; set; }

        public DateTime FechaPago { get; set; } = DateTime.Now;
    }
}
