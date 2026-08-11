using Ookii.Dialogs.Wpf;
using Microsoft.Win32;
using Sachiel.Services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.IO;

namespace Sachiel.Views
{
    public partial class ConfigView : UserControl
    {
        private readonly BackupService _backupService = new();

        public ConfigView()
        {
            InitializeComponent();
            txtCarpetaExportacion.Text = App.Settings.ExportFolder ?? "";
        }

        private void btnCrearBackup_Click(object sender, RoutedEventArgs e)
        {
            if (_backupService.CreateBackup())
                MessageBox.Show("La copia de seguridad se creó correctamente.", "Copia de seguridad", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("No se pudo crear la copia de seguridad.", "Copia de seguridad", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void btnRestaurarBackup_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Title = "Seleccionar backup", Filter = "Archivos de base de datos (*.db)|*.db",
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "Backups")};

            if (dialog.ShowDialog() != true) return;

            MessageBoxResult confirmacion = MessageBox.Show(
                "¿Está seguro de que desea restaurar esta copia de seguridad?\n\n" + "La base de datos actual será reemplazada.",
                "Copia de seguridad", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirmacion != MessageBoxResult.Yes) return;

            bool resultado = _backupService.RestoreBackup(dialog.FileName);
            if (resultado)
            {
                MessageBox.Show(
                    "La copia de seguridad se restauró correctamente.\n\n" + "Sachiel se reiniciará para aplicar los cambios.",
                    "Copia de seguridad", MessageBoxButton.OK, MessageBoxImage.Information);

                RestartApp();
            }
            else
                MessageBox.Show("No se pudo restaurar la copia de seguridad.", "Copia de seguridad", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnCambiarCarpeta_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                txtCarpetaExportacion.Text = dialog.SelectedPath;
                App.Settings.ExportFolder = dialog.SelectedPath;
                App.Settings.Save();
            }
        }

        private void RestartApp()
        {
            string exePath = Environment.ProcessPath!;
            Process.Start(exePath);
            Application.Current.Shutdown();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });
            e.Handled = true;
        }
    }
}
