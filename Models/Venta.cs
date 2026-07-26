
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sachiel.Models
{
    public enum Local { Makai, Chaska }
    public enum Metodo { Efectivo, Debito, Credito, Transferencia, QR, Otro}
    public class Venta : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal PrecioTotal { get; set; }
        public decimal Descuento { get; set; }
        public Metodo MetodoPago { get; set; }
        public Local Local { get; set; }
        public bool Cuotas { get; set; } = false;

        private bool _facturada;
        public bool Facturada { get => _facturada; set {
            if (_facturada == value) return;
            _facturada = value;
            OnPropertyChanged();
        } }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

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
