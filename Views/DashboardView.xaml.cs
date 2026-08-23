using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Win32;
using Sachiel.Services;
using Sachiel.Services.Import;
using Sachiel.ViewModels.Dashboard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sachiel.Views
{

    public partial class DashboardView : UserControl
    {
        private readonly ProductoService _productoService = new();
        private readonly VentaService _ventaService = new();
        private readonly DashboardService _dashboardService = new();

        public DashboardView() { 
            InitializeComponent();
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            DateOnly fort = DateOnly.FromDateTime(DateTime.Today).AddDays(-15);
            var productos = _productoService.GetProductos();
            var ventasFort = _ventaService.GetVentasFilter(from: fort);
            var ventasPendientes = _ventaService.GetVentasFilter(facturada: false);

            txtProductos.Text = productos.Count.ToString();
            txtVentas.Text = ventasFort.Count.ToString();

            decimal ingresos = ventasFort.Sum(v => v.PrecioTotal);
            txtIngresos.Text = ingresos.ToString("C");

            int pendientes = ventasPendientes.Count(v => !v.Facturada);
            txtPendientes.Text = pendientes.ToString();

            dgVentasRecientes.ItemsSource = ventasFort.OrderByDescending(v => v.Fecha).Take(10).ToList();

            DataContext = new DashViewModel();
        }

        private void ReloadVentas()
        {
            DateOnly fort = DateOnly.FromDateTime(DateTime.Today).AddDays(-15);
            var ventasFort = _ventaService.GetVentasFilter(from: fort);
            var ventasPendientes = _ventaService.GetVentasFilter(facturada: false);

            txtVentas.Text = ventasFort.Count.ToString();
            decimal ingresos = ventasFort.Sum(v => v.PrecioTotal);
            txtIngresos.Text = ingresos.ToString("C");
            int pendientes = ventasPendientes.Count(v => !v.Facturada);
            txtPendientes.Text = pendientes.ToString();

            dgVentasRecientes.ItemsSource = ventasFort.OrderByDescending(v => v.Fecha).Take(10).ToList();

            // DataContext refresh ????
        }

        private void CardProductos_Click(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main) main.NavigateTo(new ProductoView());
        }
        private void CardVentas_Click(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main) main.NavigateTo(new VentaView());
        }
        private void CardPendientes_Click(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main) main.NavigateTo(new VentaView(facturada: false));
        }

        private void btnNuevaVenta_Click(object sender, RoutedEventArgs e)
        {
            AddVentaView window = new();
            window.ShowDialog();
            ReloadVentas();
        }
        private void btnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main) main.NavigateTo(new ProductoView());
        }
        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            ExportVentasView window = new();
            window.ShowDialog();
        }

        private void btnImportar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new() { Filter = "Archivos Excel (*.xlsx)|*.xlsx", Title = "Seleccionar lista de precios"};
            if (dialog.ShowDialog() != true) return;

            ImportService importService = new();
            try
            {
                ImportPreview preview = importService.PreviewImport(dialog.FileName);
                ImportPreviewView window = new(importService, preview);
                if (window.ShowDialog() == true) LoadInitialData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Importar productos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
