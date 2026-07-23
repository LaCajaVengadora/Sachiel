using Sachiel.Models;
using Sachiel.Services;
using Sachiel.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Sachiel.Views
{
    public partial class VentaView : Window
    {
        private readonly ProductoService _productoService = new();
        private readonly ObservableCollection<ProductoVenta> _productosVenta = new();
        private ObservableCollection<Producto> _productosDisponibles = new();

        public VentaView()
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
            if (dpDate.SelectedDate > DateTime.Today) // PONER LO DE NOT POS TODAY
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
            if (!int.TryParse(txtDescuento.Text, out int descuento) || descuento < 0) { MessageBox.Show("Ingrese un descuento válido."); return; }
            if (cbProducto.SelectedItem is not Producto producto)
            {
                MessageBox.Show("Seleccione un producto"); return;
            }
            int cantidad = nudCantidad.Value ?? 1;
            //if (int cantidad = nudCantidad.Value ?? 1;) { MessageBox.Show("Ingrese una cantidad válida."); return; }

            ProductoVenta productoVenta = new() {
                ProductoId = producto.Id,
                Nombre = producto.Nombre,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            };

            _productosVenta.Add(productoVenta);
            _productosDisponibles.Remove(producto);
            cbProducto.Focus();
            nudCantidad.Value = 1;
            SetCartEmpty(false);
        }


        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
