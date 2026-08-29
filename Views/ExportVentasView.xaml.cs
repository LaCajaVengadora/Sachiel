using Sachiel.Models;
using System.IO;
using Sachiel.Services;
using Sachiel.Services.Export;
using System.Windows;
using Ookii.Dialogs.Wpf;
using Sachiel.ViewModels;

namespace Sachiel.Views
{
    public partial class ExportVentasView : Window
    {
        private readonly VentaService _ventaService;
        private readonly ExportService _exportService;

        public ExportVentasView(VentaService ventaService, ExportService exportService)
        {
            InitializeComponent();

            _ventaService = ventaService; _exportService = exportService;

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            dpHasta.SelectedDate = today.ToDateTime(TimeOnly.MinValue);
            dpDesde.SelectedDate = today.AddDays(-15).ToDateTime(TimeOnly.MinValue);
            txtCarpeta.Text = App.Settings.ExportFolder ?? "";
            txtArchivo.Text = $"Ventas_{today:yyMMdd}";
        }
        public void SetFilter(Filter filtro)
        {
            InitializeComponent();

            if (filtro.Locales.Contains(Local.Makai)) tgMakai.IsChecked = true;
            if (filtro.Locales.Contains(Local.Chaska)) tgChaska.IsChecked = true;

            dpDesde.SelectedDate = filtro.From?.ToDateTime(TimeOnly.MinValue);
            dpHasta.SelectedDate = filtro.To?.ToDateTime(TimeOnly.MinValue);
            txtArchivo.Text = $"Ventas_{filtro.To:yyMMdd}";

            if (filtro.Metodos.Contains(Metodo.Efectivo)) tgEfectivo.IsChecked = true;
            if (filtro.Metodos.Contains(Metodo.Debito)) tgDebito.IsChecked = true;
            if (filtro.Metodos.Contains(Metodo.Credito)) tgCredito.IsChecked = true;
            if (filtro.Metodos.Contains(Metodo.Transferencia)) tgTransferencia.IsChecked = true;
            if (filtro.Metodos.Contains(Metodo.QR)) tgQR.IsChecked = true;
            if (filtro.Metodos.Contains(Metodo.Otro)) tgOtro.IsChecked = true;

            if (filtro.Facturada.HasValue)
            {
                if (filtro.Facturada == true) rbSi.IsChecked = true;
                else rbNo.IsChecked = true;
            }

        }

        private void btnExaminar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true) txtCarpeta.Text = dialog.SelectedPath;
        }

        private bool Validate()
        {
            if (dpDesde.SelectedDate == null || dpHasta.SelectedDate == null || dpDesde.SelectedDate > dpHasta.SelectedDate)
            {
                MessageBox.Show("Seleccione un rango de fechas válido", "Exportar ventas", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else if (string.IsNullOrWhiteSpace(txtCarpeta.Text))
            {
                MessageBox.Show("Seleccione una carpeta de destino", "Exportar ventas", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else if (string.IsNullOrWhiteSpace(txtArchivo.Text))
            {
                MessageBox.Show("Ingrese un nombre para el archivo", "Exportar ventas", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else return true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => Close();

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            List<Local> locales = [];
            if (tgMakai.IsChecked == true) locales.Add(Local.Makai);
            if (tgChaska.IsChecked == true) locales.Add(Local.Chaska);

            DateOnly desde = DateOnly.FromDateTime(dpDesde.SelectedDate!.Value); // CANT BE NULL CUZ VALIDATE
            DateOnly hasta = DateOnly.FromDateTime(dpHasta.SelectedDate!.Value);

            List<Metodo> metodos = [];
            if (tgEfectivo.IsChecked == true) metodos.Add(Metodo.Efectivo);
            if (tgDebito.IsChecked == true) metodos.Add(Metodo.Debito);
            if (tgCredito.IsChecked == true) metodos.Add(Metodo.Credito);
            if (tgTransferencia.IsChecked == true) metodos.Add(Metodo.Transferencia);
            if (tgQR.IsChecked == true) metodos.Add(Metodo.QR);
            if (tgOtro.IsChecked == true) metodos.Add(Metodo.Otro);

            bool? facturada = null;
            if (rbSi.IsChecked == true) facturada = true;
            if (rbNo.IsChecked == true) facturada = false;

            var ventas = _ventaService.GetVentasFilter(new Filter() { From=desde, To=hasta, Locales=locales, Metodos=metodos, Facturada=facturada });
            if (ventas.Count == 0)
            {
                MessageBox.Show("No se han encontrado ventas para exportar", "Exportar ventas", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ExportOptions options = new()
            {
                Format = rbExcel.IsChecked == true ? ExportFormat.Excel : ExportFormat.Pdf,
                From = desde,// CANT BE NULL CUZ VALIDATE
                To = hasta,
                OutputPath = Path.Combine(txtCarpeta.Text,txtArchivo.Text + (rbExcel.IsChecked == true ? ".xlsx" : ".pdf")),
                IncludeDetails = chkDetalles.IsChecked == true
            };

            string localesTxt = locales == null ? "Todos" : string.Join(", ", locales);
            string metodosTxt = metodos == null ? "Todos" : string.Join(", ", metodos);
            string facturacionTxt = facturada switch { true => "Solo facturadas", false => "Solo sin facturar", null => "Todas"};
            string preview = string.Join(
                Environment.NewLine,
                ventas.Take(10).Select(v =>
                    $"{v.Id:X4} | {v.Fecha:yyMMdd} | {v.Local} | {v.ResumenProductos} | {v.PrecioTotal:C}")
            );
            if (ventas.Count > 10)
            {
                preview += $"{Environment.NewLine}...";
                preview += $"{Environment.NewLine}({ventas.Count - 10} ventas más)";
            }
            string f = rbPdf.IsChecked == true ? "Pdf" : "Excel";
            string mensaje =
                $"Se exportarán {ventas.Count} ventas en formato {f}. \n\n" +

                $"Filtros aplicados\n" +
                $"──────────────────────\n" +
                $"Período: {options.From:yyMMdd} - {options.To:yyMMdd}\n" +
                $"Locales: {localesTxt}\n" +
                $"Métodos: {metodosTxt}\n" +
                $"Facturación: {facturacionTxt}\n" +
                $"Detalle de productos: {(options.IncludeDetails ? "Sí" : "No")}\n\n" +

                $"Vista previa\n" +
                $"──────────────────────\n" +
                $"{preview}\n\n" +

                $"¿Desea continuar?";
            MessageBoxResult result = MessageBox.Show(
                mensaje,
                "Confirmar exportación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            
            if (result != MessageBoxResult.Yes) return;

            if (_exportService.Export(ventas, options)) MessageBox.Show("Exportación realizada correctamente", "Exportar ventas", MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show("No se pudo realizar la exportación", "Exportar ventas", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}