using Microsoft.Win32;
using Sachiel.Services;
using Sachiel.Services.Import;
using Sachiel.ViewModels;
using Sachiel.ViewModels.Dashboard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sachiel.Views
{

    public partial class DashboardView : UserControl
    {
        private readonly ProductoService _productoService; 
        private readonly VentaService _ventaService;
        private readonly ImportService _importService;
        private readonly DashViewModel _viewModel;
        private readonly IViewFactory _viewFactory;


        public DashboardView(ProductoService productoService, VentaService ventaService, ImportService importService,
            DashViewModel viewModel, IViewFactory viewFactory) 
        { 
            InitializeComponent(); 
            _productoService = productoService; _ventaService = ventaService; _importService = importService;
            _viewModel = viewModel; _viewFactory = viewFactory;
            DataContext = _viewModel;
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            var productos = _productoService.GetProductos();
            LoadVentas();
            txtProductos.Text = productos.Count.ToString();
        }

        private void LoadVentas()
        {
            DateOnly fort = DateOnly.FromDateTime(DateTime.Today).AddDays(-15);
            var ventasFort = _ventaService.GetVentasFilter(new Filter() { From = fort });
            var ventasPendientes = _ventaService.GetVentasFilter(new Filter() { Facturada = false });

            txtVentas.Text = ventasFort.Count.ToString();
            decimal ingresos = ventasFort.Sum(v => v.PrecioTotal);
            txtIngresos.Text = ingresos.ToString("C");
            int pendientes = ventasPendientes.Count(v => !v.Facturada);
            txtPendientes.Text = pendientes.ToString();

            dgVentasRecientes.ItemsSource = ventasFort.OrderByDescending(v => v.Fecha).Take(10).ToList();

            _viewModel.Refresh();
        }

        private void CardProductos_Click(object sender, EventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main) main.NavigateTo(_viewFactory.Create<ProductoView>());
        }
        private void CardVentas_Click(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main) main.NavigateTo(_viewFactory.Create<VentaView>());
        }
        private void CardPendientes_Click(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main) main.NavigateTo(_viewFactory.Create<VentaView>(false));
        }

        private void btnNuevaVenta_Click(object sender, RoutedEventArgs e)
        {
            AddVentaView window = _viewFactory.Create<AddVentaView>();
            window.ShowDialog();
            LoadVentas();
        }
        private void btnNuevoProducto_Click(object sender, RoutedEventArgs e) => CardProductos_Click(sender, e);
        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            ExportVentasView window = _viewFactory.Create<ExportVentasView>();
            window.ShowDialog();
        }

        private void btnImportar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new() { Filter = "Archivos Excel (*.xlsx)|*.xlsx", Title = "Seleccionar lista de precios" };
            if (dialog.ShowDialog() != true) return;

            try
            {
                var window = _viewFactory.Create<ImportPreviewView>();
                window.SetPreview(_importService.PreviewImport(dialog.FileName));
                if (window.ShowDialog() == true) LoadInitialData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Importar productos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
