using System;

namespace Sachiel.Models
{
    public enum Local { Makai, Chaska }
    public class Venta
    {
        public int Id { get; set; }
        public string CodigoVenta { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public decimal PrecioTotal { get; set; }
        public Local Local { get; set; }
        public bool Facturada { get; set; } = false;

        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
