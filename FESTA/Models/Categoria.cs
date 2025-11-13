using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FESTA.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }

        // 🖼️ Imagen grande tipo banner
        public string? ImagenUrl { get; set; }
        // Relación recursiva para subcategorías
        public int? CategoriaPadreId { get; set; }
        public Categoria? CategoriaPadre { get; set; }

        // Relación: una categoría tiene varios productos
        public ICollection<Producto>? Productos { get; set; }
        [NotMapped] 
        public IFormFile? ImagenArchivo { get; set; }
        public ICollection<Categoria>? Subcategorias { get; set; }
    }
}
