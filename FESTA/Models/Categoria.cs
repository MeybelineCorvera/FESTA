using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public List<Categoria> Subcategorias { get; set; } = new();

        // Relación: una categoría tiene varios productos
        public ICollection<Producto>? Productos { get; set; }
    }
}
