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

using Sachiel.Models;
using Sachiel.Services;

namespace Sachiel.Views
{
    public partial class ProductoView : Window
    {
        private readonly ProductoService _productoService = new();
        public ProductoView()
        { 
            InitializeComponent();
            LoadProductos();
        }

        private void LoadProductos()
        {
            dgProductos.ItemsSource = _productoService.GetProductos();
        }
    }
}
