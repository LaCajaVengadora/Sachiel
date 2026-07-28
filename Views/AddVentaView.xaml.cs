using Sachiel.Models;
using Sachiel.Services;
using Sachiel.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Sachiel.Views
{
    public partial class AddVentaView : Window
    {
        private decimal _total = 0;
        private readonly VentaService _ventaService = new();
        private readonly ProductoService _productoService = new();
        private readonly ObservableCollection<ProductoVenta> _productosVenta = new();
        private ObservableCollection<Producto> _productosDisponibles = new();

        public AddVentaView()
        {
            InitializeComponent();
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            dpDate.SelectedDate = DateTime.Today;
            cbLocal.ItemsSource = Enum.GetValues<Local>();
            cbPago.ItemsSource = Enum.GetValues<Metodo>();
            txtDescuento.Text = "0";
            nudCantidad.Value = 1;

            _productosDisponibles = new ObservableCollection<Producto>(_productoService.GetProductos());
            cbProducto.ItemsSource = _productosDisponibles;
            cbProducto.DisplayMemberPath = "Nombre";

            dgProductos.ItemsSource = _productosVenta;
            SetCartEmpty();
        }

        private void SetCartEmpty(bool empty = true)
        {
            if (empty)
            {
                lEmpty.Visibility = Visibility.Visible;
                gDetalleVenta.Visibility = Visibility.Collapsed;
            }
            else
            {
                lEmpty.Visibility = Visibility.Collapsed;
                gDetalleVenta.Visibility = Visibility.Visible;
            }
        }

        private void cbPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPago.SelectedItem is Metodo metodo && metodo == Metodo.Credito)
                chkCuotas.Visibility = Visibility.Visible;
            else
            {
                chkCuotas.IsChecked = false;
                chkCuotas.Visibility = Visibility.Collapsed;
            }
        }
        
        private void btnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (cbProducto.SelectedItem is not Producto producto)
            {
                MessageBox.Show("Seleccione un producto"); return;
            }
            int cantidad = nudCantidad.Value ?? 1;
            //if (int cantidad = nudCantidad.Value ?? 1;) { MessageBox.Show("Ingrese una cantidad válida."); return; }

            ProductoVenta productoVenta = new() {
                Producto = producto,
                Cantidad = cantidad
            };

            _productosVenta.Add(productoVenta);
            _productosDisponibles.Remove(producto);
            cbProducto.Focus();
            nudCantidad.Value = 1;
            RecalcularTotales();
            SetCartEmpty(false);
        }

        private void RecalcularTotales()
        {
            decimal subtotal = _productosVenta.Sum(p => p.Subtotal);

            decimal descuento = 0;
            decimal.TryParse(txtDescuento.Text, out descuento);

            decimal baseCalculo = subtotal - descuento;
            if (baseCalculo < 0) baseCalculo = 0;

            decimal recargo = 0;
            if (chkCuotas.IsChecked == true) recargo = baseCalculo * 0.25m;

            _total = baseCalculo + recargo;

            lbSubtotal.Content = subtotal.ToString("C");
            lbDescuentoFinal.Content = descuento.ToString("C");
            lbRecargo.Content = recargo.ToString("C");
            lbTotal.Content = _total.ToString("C");
        }

        private void dataChanged(object sender, EventArgs e)
        {
            RecalcularTotales();
        }

        private void MenuEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem is not ProductoVenta productoVenta)
            {
                MessageBox.Show("Seleccione un producto."); return;
            }

            Producto producto = productoVenta.Producto;
            int idx = _productosDisponibles
                .TakeWhile(p => string.Compare(p.Nombre, producto.Nombre, StringComparison.CurrentCultureIgnoreCase) < 0)
                .Count();
            _productosDisponibles.Insert(idx, producto);

            _productosVenta.Remove(productoVenta);

            SetCartEmpty(_productosVenta.Count == 0);
            RecalcularTotales();

        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (dpDate.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Seleccione una fecha válida."); return;
            }
            if (cbLocal.SelectedItem is not Local)
            {
                MessageBox.Show("Seleccione un local."); return;
            }
            if (cbPago.SelectedItem is not Metodo)
            {
                MessageBox.Show("Seleccione un método de pago."); return;
            }

            if ((chkCuotas.IsChecked == true) && ((Metodo)cbPago.SelectedItem == Metodo.Credito)) // XNOR
            {
                MessageBox.Show("Error, ingrese nuevamente el método de pago."); return;
            }
            decimal descuento = 0;
            if (!string.IsNullOrWhiteSpace(txtDescuento.Text) &&
                (!decimal.TryParse(txtDescuento.Text, out descuento) || descuento < 0))
            {
                MessageBox.Show("Ingrese un descuento válido."); return;
            }
            if (_productosVenta.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto.");
                return;
            }

            Venta venta = new()
            {
                Fecha = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                Local = (Local)cbLocal.SelectedItem,
                MetodoPago = (Metodo)cbPago.SelectedItem,
                Cuotas = chkCuotas.IsChecked == true,
                Descuento = descuento,
                PrecioTotal = _total,
                Facturada = (Metodo)cbPago.SelectedItem == Metodo.Efectivo
            };

            bool added = _ventaService.AddVenta(venta, _productosVenta);
            if (!added)
            {
                MessageBox.Show("No se pudo registrar la venta."); return;
            }
            Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (_productosVenta.Count > 0)
            {
                var result = MessageBox.Show(
                    "Se perderán los cambios realizados. ¿Desea salir?",
                    "Cancelar venta",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No) return;
            }
            Close();
        }

    }
}
