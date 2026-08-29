using Sachiel.Services;
using Sachiel.Views;
using System.Windows;
using System.Windows.Controls;

namespace Sachiel
{
    public partial class MainWindow : Window
    {
        private bool _sidebarExpanded = true;
        private readonly IViewFactory _viewFactory;

        public MainWindow(IViewFactory viewFactory)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            _viewFactory = viewFactory;
            NavigateTo(_viewFactory.Create<DashboardView>());
        }
        public void NavigateTo(UserControl view) => MainContent.Content = view;

        private void btnInicio_Click(object sender, RoutedEventArgs e) => NavigateTo(_viewFactory.Create<DashboardView>());
        private void btnVentas_Click(object sender, RoutedEventArgs e) => NavigateTo(_viewFactory.Create<VentaView>());
        private void btnProductos_Click(object sender, RoutedEventArgs e) => NavigateTo(_viewFactory.Create<ProductoView>());
        private void btnConfig_Click(object sender, RoutedEventArgs e) => NavigateTo(_viewFactory.Create<ConfigView>());


        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (_sidebarExpanded)
            {
                Sidebar.Width = new GridLength(80);

                logo.Height = 40; logo.Width = 40; logo.Margin = new Thickness(15, 30, 15, 30);
                txtInicio.Visibility = Visibility.Collapsed; btnInicio.Margin = new Thickness(10, 0, 10, 15);
                txtVentas.Visibility = Visibility.Collapsed; btnVentas.Margin = new Thickness(10, 0, 10, 15);
                txtProductos.Visibility = Visibility.Collapsed; btnProductos.Margin = new Thickness(10, 0, 10, 15);
                txtConfig.Visibility = Visibility.Collapsed; btnConfig.Margin = new Thickness(10, 0, 10, 15);
                                                                
                btnCollapse.Content = "→";

                _sidebarExpanded = false;
            }
            else
            {
                Sidebar.Width = new GridLength(220);

                logo.Height = 100; logo.Width = 100; logo.Margin = new Thickness(30);
                txtInicio.Visibility = Visibility.Visible; btnInicio.Margin = new Thickness(20,0,20,15);
                txtVentas.Visibility = Visibility.Visible; btnVentas.Margin = new Thickness(20, 0, 20, 15);
                txtProductos.Visibility = Visibility.Visible; btnProductos.Margin = new Thickness(20, 0, 20, 15);
                txtConfig.Visibility = Visibility.Visible; btnConfig.Margin = new Thickness(20, 0, 20, 15);

                btnCollapse.Content = "←";

                _sidebarExpanded = true;
            }
        }
    }
}