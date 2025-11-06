using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FESTA.Models
{
    public class DetalleReserva
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Reserva")]
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }

        [ForeignKey("Producto")]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }
}
