using Sachiel.Models;
using System.IO;
using Sachiel.Services;
using Sachiel.Services.Export;
using System.Windows;
using Ookii.Dialogs.Wpf;
using Sachiel.Services.Import;

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
        }


        private void btnImportar_Click(object sender, RoutedEventArgs e)
        {
            if (_importService.ApplyImport(_preview))
            {
                MessageBox.Show("La importación se realizó correctamente.", "Importación", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else MessageBox.Show("No se pudo completar la importación.", "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
        }
    }
}