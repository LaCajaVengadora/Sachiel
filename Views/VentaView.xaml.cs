using Sachiel.Models;
using Sachiel.Services;
using Sachiel.ViewModels;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sachiel.Views
{
    public partial class VentaView : UserControl
    {
        private readonly VentaService _ventaService = new();
        private ObservableCollection<Venta> _ventasSeleccionadas = new();
        private List<Venta> _ventasRecientes = new();
        private DataGridRow? _filaExpandida;

        public VentaView()
        {
            InitializeComponent();
            LoadInitialData();
        }
        public VentaView(bool facturada)
        {
            InitializeComponent();

            dpHasta.SelectedDate = DateTime.Today;
            dpDesde.SelectedDate = DateTime.Today.AddDays(-15);

            _ventasRecientes = _ventaService.GetVentasFilter(new Filter());
            _ventasSeleccionadas = new ObservableCollection<Venta>();
            dgListadoVentas.ItemsSource = _ventasSeleccionadas;

            if (facturada) rbSi.IsChecked = true;
            else rbNo.IsChecked = true;

            var ventas = _ventaService.GetVentasFilter(new Filter() { Facturada=facturada });
            _ventasSeleccionadas.Clear();
            foreach (var venta in ventas) _ventasSeleccionadas.Add(venta);
        }

        private void LoadInitialData()
        {
            dpHasta.SelectedDate = DateTime.Today;
            dpDesde.SelectedDate = DateTime.Today.AddDays(-15);

            _ventasRecientes = _ventaService.GetVentasFilter(new Filter());
            _ventasSeleccionadas = new ObservableCollection<Venta>(_ventasRecientes);
            dgListadoVentas.ItemsSource = _ventasSeleccionadas;
        }
        private void ReloadData()
        {
            CerrarDetalle();
            _ventasRecientes = _ventaService.GetVentasFilter(new Filter());
            _ventasSeleccionadas.Clear();
            foreach (var venta in _ventasRecientes) _ventasSeleccionadas.Add(venta);
        }
        
        private void btnNuevaVenta_Click(object sender, RoutedEventArgs e)
        {
            AddVentaView window = new();
            window.ShowDialog();
            ReloadData();
        }

        private void AbrirDetalle(Venta venta)
        {
            dgListadoVentas.ScrollIntoView(venta); dgListadoVentas.UpdateLayout();

            DataGridRow? fila = (DataGridRow?)dgListadoVentas.ItemContainerGenerator.ContainerFromItem(venta);
            if (fila == null) return;

            CerrarDetalle();

            fila.DetailsVisibility = Visibility.Visible; fila.IsSelected = true;
            _filaExpandida = fila;
        }
        private void CerrarDetalle()
        {
            _filaExpandida?.DetailsVisibility = Visibility.Collapsed;
            _filaExpandida = null;
            dgListadoVentas.SelectedItem = null;
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string filtro = txtBuscar.Text.Trim();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                txtBuscar.Focus(); return;
            }
            if (!int.TryParse(filtro, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int id))
            {
                MessageBox.Show("Ingrese un código válido", "Buscar venta", MessageBoxButton.OK, MessageBoxImage.Information); return;
            }

            Venta? venta = _ventaService.GetVenta(id);
            if (venta == null)
            {
                MessageBox.Show($"No se encontró ninguna venta con ID {id:X4}", "Buscar venta", MessageBoxButton.OK, MessageBoxImage.Warning); return;
            }

            CerrarDetalle();
            _ventasSeleccionadas.Clear();
            _ventasSeleccionadas.Add(venta);
            AbrirDetalle(venta);
        }

        private void dgListadoVentas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject? source = e.OriginalSource as DependencyObject;
            while (source != null && source is not DataGridRow) source = VisualTreeHelper.GetParent(source);
            if (source is not DataGridRow fila)
            {
                CerrarDetalle(); return;
            }

            Venta venta = (Venta)fila.Item;
            if (_filaExpandida == fila) CerrarDetalle();
            else AbrirDetalle(venta);
            e.Handled = true;
        }
        
        private Filter BuildFilter()
        {
            List<Local> locales = [];
            if (tgMakai.IsChecked == true) locales.Add(Local.Makai);
            if (tgChaska.IsChecked == true) locales.Add(Local.Chaska);

            DateOnly? desde = null; DateOnly? hasta = null;
            if (dpDesde.SelectedDate.HasValue) desde = DateOnly.FromDateTime(dpDesde.SelectedDate.Value);
            if (dpHasta.SelectedDate.HasValue) hasta = DateOnly.FromDateTime(dpHasta.SelectedDate.Value);

            List<Metodo> metodos = [];
            if (tgEfectivo.IsChecked == true) metodos.Add(Metodo.Efectivo);
            if (tgDebito.IsChecked == true) metodos.Add(Metodo.Debito);
            if (tgCredito.IsChecked == true) metodos.Add(Metodo.Credito);
            if (tgTransferencia.IsChecked == true) metodos.Add(Metodo.Transferencia);
            if (tgQR.IsChecked == true) metodos.Add(Metodo.QR);
            if (tgOtro.IsChecked == true) metodos.Add(Metodo.Otro);

            bool? facturada = null;
            if (rbSi.IsChecked == true) facturada = true;
            if (rbNo.IsChecked == true) facturada = false;

            return new Filter() { Locales = locales, From = desde, To = hasta, Metodos = metodos, Facturada = facturada };
        }


        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            var ventas = _ventaService.GetVentasFilterACTUAL(BuildFilter());

            if (ventas.Count == 0)
            {
                MessageBox.Show("No se han encontrado ventas", "Filtrar ventas", MessageBoxButton.OK, MessageBoxImage.Warning); return;
            }

            CerrarDetalle();
            _ventasSeleccionadas.Clear();
            foreach (var venta in ventas) _ventasSeleccionadas.Add(venta);
        }
        
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            Filter filtro = BuildFilter();
            var ventas = _ventaService.GetVentasFilterACTUAL(filtro);

            if (ventas.Count == 0)
            {
                MessageBox.Show("No se han encontrado ventas", "Exportar ventas", MessageBoxButton.OK, MessageBoxImage.Warning); return;
            }

            ExportVentasView window = new(ventas, filtro);
            window.ShowDialog();

        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarAll();
        }

        private void LimpiarAll()
        {
            txtBuscar.Clear();

            tgMakai.IsChecked = false; tgChaska.IsChecked = false;
            dpDesde.SelectedDate = null; dpHasta.SelectedDate = null;
            tgEfectivo.IsChecked = false; tgDebito.IsChecked = false; tgCredito.IsChecked = false; tgTransferencia.IsChecked = false; tgQR.IsChecked = false; tgOtro.IsChecked = false;
            rbTodas.IsChecked = true; rbSi.IsChecked = false; rbNo.IsChecked = false;

            ReloadData();
        }

        private void MenuFacturada_Click(object sender, RoutedEventArgs e) 
        {
            if (dgListadoVentas.SelectedItem is not Venta venta)
            {
                MessageBox.Show("Seleccione una venta", "Marcar facturada", MessageBoxButton.OK, MessageBoxImage.Information); return;
            }

            bool nuevoEstado = !venta.Facturada;
            if (!_ventaService.UpdateVentaFacturada(venta.Id, nuevoEstado))
            {
                MessageBox.Show("No se ha podido marcar la facturación", "Marcar facturada", MessageBoxButton.OK, MessageBoxImage.Error); return;
            }

            venta.Facturada = nuevoEstado;
            btnFiltrar_Click(sender, e);
            
        }

        private void MenuEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgListadoVentas.SelectedItem is not Venta venta)
            {
                MessageBox.Show("Seleccione una venta", "Eliminar venta", MessageBoxButton.OK, MessageBoxImage.Information); return;
            }

            string detalleProductos = string.Join("\n",
                venta.Detalles.Take(5).Select(d => $"- {d.Producto.Nombre} x{d.Cantidad} (${d.PrecioUnitario:N2})"));
            if (venta.Detalles.Count > 5) detalleProductos += $"\n... y {venta.Detalles.Count - 5} productos más.";

            MessageBoxResult result = MessageBox.Show(
                $"¿Desea eliminar la venta con ID '{venta.Id:X4}'?\n\n" +
                    $"Productos:\n{detalleProductos}\n\n" +
                    $"Total: ${venta.PrecioTotal:N2}",
                "Eliminar venta",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes) return;

            bool deleted = _ventaService.DeleteVenta(venta.Id);
            if (!deleted)
            {
                MessageBox.Show("Ha ocurrido un error al eliminar la venta", "Eliminar venta", MessageBoxButton.OK, MessageBoxImage.Error); return;
            }

            ReloadData();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (dgListadoVentas.SelectedItem is Venta venta)
                MenuFacturada.Header = venta.Facturada ? "Desmarcar facturada" : "Marcar facturada";
        }
    }
}
