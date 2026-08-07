using Sachiel.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sachiel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _sidebarExpanded = true;

        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            NavigateTo(new DashboardView());
        }
        public void NavigateTo(UserControl view) => MainContent.Content = view;

        private void btnInicio_Click(object sender, RoutedEventArgs e) => NavigateTo(new DashboardView());
        private void btnVentas_Click(object sender, RoutedEventArgs e) => NavigateTo(new VentaView());
        private void btnProductos_Click(object sender, RoutedEventArgs e) => NavigateTo(new ProductoView());
        private void btnConfig_Click(object sender, RoutedEventArgs e) => NavigateTo(new ConfigView());

        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (_sidebarExpanded)
            {
                Sidebar.Width = new GridLength(80);

                //logo.Visibility = Visibility.Collapsed;

                btnInicio.Content = "🏠"; btnInicio.Margin = new Thickness(10,0,10,15);
                btnVentas.Content = "🛒"; btnVentas.Margin = new Thickness(10, 0, 10, 15);
                btnProductos.Content = "📦"; btnProductos.Margin = new Thickness(10, 0, 10, 15);
                btnConfig.Content = "⚙"; btnConfig.Margin = new Thickness(10, 0, 10, 15);

                btnCollapse.Content = "→";

                _sidebarExpanded = false;
            }
            else
            {
                Sidebar.Width = new GridLength(220);

                //logo.Visibility = Visibility.Visible;

                btnInicio.Content = "🏠 Inicio"; btnInicio.Margin = new Thickness(20,0,20,15);
                btnVentas.Content = "🛒 Ventas"; btnVentas.Margin = new Thickness(20, 0, 20, 15);
                btnProductos.Content = "📦 Productos"; btnProductos.Margin = new Thickness(20, 0, 20, 15);
                btnConfig.Content = "⚙ Configuración"; btnConfig.Margin = new Thickness(20, 0, 20, 15);

                btnCollapse.Content = "←";

                _sidebarExpanded = true;
            }
        }
    }
}