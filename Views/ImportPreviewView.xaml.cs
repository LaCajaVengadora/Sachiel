using Sachiel.Services.Import;
using System.Windows;

namespace Sachiel.Views
{
    public partial class ImportPreviewView : Window
    {
        private readonly ImportService _importService;
        private readonly ImportPreview _preview;

        public ImportPreviewView(ImportService service, ImportPreview preview)
        {
            InitializeComponent();

            _importService = service; _preview = preview;
            LoadInitialData();
        }


        private void LoadInitialData()
        {
            txtNuevos.Text = _preview.Nuevos.Count.ToString();
            txtActualizados.Text = _preview.Actualizados.Count.ToString();
            txtSinCambios.Text = _preview.SinCambios.ToString();

            dgNuevos.ItemsSource = _preview.Nuevos;
            dgActualizados.ItemsSource = _preview.Actualizados;

            if (_preview.Nuevos.Count == 0)
            {
                gbNuevos.Visibility = Visibility.Collapsed;
                LayoutRoot.RowDefinitions[1].Height = new GridLength(0);
            }

            if (_preview.Actualizados.Count == 0)
            {
                gbActualizados.Visibility = Visibility.Collapsed;
                LayoutRoot.RowDefinitions[2].Height = new GridLength(0);
            }

            btnImportar.IsEnabled = _preview.Nuevos.Count != 0  || _preview.Actualizados.Count != 0;
        }

        private void btnImportar_Click(object sender, RoutedEventArgs e)
        {
            if (_importService.ApplyImport(_preview))
            {
                MessageBox.Show("La importación se realizó correctamente.", "Importar productos", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else MessageBox.Show("No se pudo completar la importación.", "Importar productos", MessageBoxButton.OK, MessageBoxImage.Error); 
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
        }
    }
}