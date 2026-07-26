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
            ShowInicio();
        }
        private void ShowInicio()
            { MainContent.Content = new DashboardView(); }

        private void btnInicio_Click(object sender, RoutedEventArgs e) 
            { ShowInicio(); }
        private void btnVentas_Click(object sender, RoutedEventArgs e)
            { MainContent.Content = new VentaView(); }
        private void btnProductos_Click(object sender, RoutedEventArgs e)
            { MainContent.Content = new ProductoView(); }
        private void btnConfig_Click(object sender, RoutedEventArgs e)
            { MainContent.Content = new ConfigView(); }

        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (_sidebarExpanded)
            {
                Sidebar.Width = new GridLength(60);

                logo.Visibility = Visibility.Collapsed;

                btnInicio.Content = "🏠";
                btnVentas.Content = "🛒";
                btnProductos.Content = "📦";
                btnConfig.Content = "⚙";

                btnCollapse.Content = "→";

                _sidebarExpanded = false;
            }
            else
            {
                Sidebar.Width = new GridLength(220);

                logo.Visibility = Visibility.Visible;

                btnInicio.Content = "🏠 Inicio";
                btnVentas.Content = "🛒 Ventas";
                btnProductos.Content = "📦 Productos";
                btnConfig.Content = "⚙ Configuración";

                btnCollapse.Content = "←";

                _sidebarExpanded = true;
            }
        }
    }
}