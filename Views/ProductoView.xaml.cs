using Sachiel.Models;
using Sachiel.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Sachiel.Views
{
    public partial class ProductoView : UserControl
    {
        private readonly ProductoService _productoService = new();
        private Producto? _productoEditando = null;
        private bool _modoEdicion = false;

        public ProductoView()
        { 
            InitializeComponent();
            LoadProductos();
        }

        private void LoadProductos()
        {
            dgProductos.ItemsSource = _productoService.GetProductos();
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese un nombre válido."); return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Ingrese un precio válido."); return; 
            }
            if (precio <=0)
            {
                MessageBox.Show("El precio debe ser mayor a 0."); return;
            }

            if (!_modoEdicion) 
            {
                Producto producto = new() { Nombre = nombre, Precio = precio};

                bool added = _productoService.AddProducto(producto);
                if (!added)
                {
                    MessageBox.Show("Ha ocurrido un error al añadir el producto."); return;
                }
            }
            else
            {
                if (_productoEditando == null) 
                {
                    MessageBox.Show("Ha ocurrido un error, seleccione un producto.");
                }
                else if (_productoEditando.Nombre == nombre && _productoEditando.Precio == precio) 
                {
                    MessageBox.Show("No se ha modificado el producto.");
                }
                else{
                    Producto nuevoProducto = new() { Id = _productoEditando.Id, Nombre = nombre, Precio = precio };
                    bool updated = _productoService.UpdateProducto(nuevoProducto);
                    if (!updated)
                    {
                        MessageBox.Show("Ha ocurrido un error al actualizar el producto.");
                    }
                }
                SalirModoEdicion();
            }

            LoadProductos();
            LimpiarCampos();

        }
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            if (_modoEdicion) SalirModoEdicion();
            LimpiarCampos();
        }
        
        private void LimpiarCampos()
        {
            txtNombre.Clear(); txtPrecio.Clear(); txtNombre.Focus();
        }
        private void SalirModoEdicion()
        {
            _productoEditando = null;
            _modoEdicion = false;
            btnAgregar.Content = "Añadir";
            btnLimpiar.Content = "Limpiar campos";
            titulo.Header = "Añadir nuevo producto";
        }

        private void MenuModificar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem is not Producto producto)
            {
                MessageBox.Show("Seleccione un producto."); return;
            }

            _modoEdicion = true;
            _productoEditando = producto;
            txtNombre.Text = producto.Nombre;
            txtPrecio.Text = producto.Precio.ToString();
            btnAgregar.Content = "Guardar cambios";
            btnLimpiar.Content = "Descartar cambios";
            titulo.Header = $"Modificar producto con ID \'{producto.Id:X3}\'";
            txtNombre.Focus();
        }

        private void MenuEliminar_Click(object sender, RoutedEventArgs e) 
        {
            if (dgProductos.SelectedItem is not Producto producto)
            {
                MessageBox.Show("Seleccione un producto."); return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"¿Desea eliminar el producto \"{producto.Nombre}\" (ID \'{producto.Id:X3}\')?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result != MessageBoxResult.Yes) return;

            bool deleted = _productoService.DeleteProducto(producto.Id);
            if (!deleted)
            {
                MessageBox.Show("Ha ocurrido un error al eliminar el producto."); return;
            }

            SalirModoEdicion();
            LoadProductos();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string filtro = txtBuscar.Text.Trim();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                txtBuscar.Focus(); return;
            }

            var productos = dgProductos.ItemsSource as List<Producto>;
            Producto? producto;
            if (int.TryParse(filtro, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int id))
            {
                producto = productos?.FirstOrDefault(p => p.Id == id);
            } else producto = productos?.OrderBy(p => p.Nombre)
                    .FirstOrDefault(p => p.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase));

            if (producto == null)
            {
                MessageBox.Show("No se encontró ningún producto."); return;
            }

            dgProductos.ScrollIntoView(producto);
            dgProductos.SelectedItem = producto;
        }
    }
}
