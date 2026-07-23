using Sachiel.Models;

namespace Sachiel.ViewModels
{
    public class ProductoVenta
    {
        public Producto Producto { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal Subtotal => Cantidad * Producto.Precio;

    }
}
