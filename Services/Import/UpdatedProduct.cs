using Sachiel.Models;

namespace Sachiel.Services.Import
{
    public class UpdatedProduct
    {
        public Producto Producto { get; set; } = null!;
        public decimal PrecioAnterior { get; set; }
        public decimal PrecioNuevo { get; set; }
    }
}
