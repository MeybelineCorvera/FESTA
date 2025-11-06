namespace FESTA.Models
{
    public class Carrito
    {
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public decimal Total => Productos.Sum(p => p.Precio);

        public void AgregarProducto(Producto producto) => Productos.Add(producto);

        public void EliminarProducto(int id)
            => Productos.RemoveAll(p => p.Id == id);

        public void Vaciar() => Productos.Clear();
    }
}
