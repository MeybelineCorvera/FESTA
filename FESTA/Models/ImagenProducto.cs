using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FESTA.Models
{
    public class ImagenProducto
    {
        [Key]
        public int Id { get; set; }

        public string Url { get; set; }

        [ForeignKey("Producto")]
        public int ProductoId { get; set; }

        public Producto Producto { get; set; }
    }
}
