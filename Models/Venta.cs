using System;

namespace Sachiel.Models
{
    public enum Local { Makai, Chaska }
    public enum Metodo { Efectivo, Debito, Credito, Transferencia, QR, Otro}
    public class Venta
    {
        public int Id { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal PrecioTotal { get; set; }
        public decimal Descuento { get; set; }
        public Metodo MetodoPago { get; set; }
        public Local Local { get; set; }
        public bool Cuotas { get; set; } = false;
        public bool Facturada { get; set; } = false;

        public string ResumenProductos
        {
            get
            {
                if (Detalles.Count == 0) return "";
                else if (Detalles.Count == 1)
                {
                    var d = Detalles.First();
                    return $"{d.Producto.Nombre} ×{d.Cantidad}";
                }
                else return $"{Detalles.First().Producto.Nombre} +{Detalles.Count - 1} más";
            }
        }

        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
