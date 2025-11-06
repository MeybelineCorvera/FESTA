using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FESTA.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public string? ImagenUrl { get; set; }

        [ForeignKey("Categoria")]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
