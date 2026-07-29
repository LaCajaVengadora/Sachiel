using Sachiel.Models;

namespace Sachiel.Services.Export
{
    public class ExportService
    {
        public bool Export(List<Venta> ventas, ExportOptions options)
        {
            IExportService<Venta> exporter = options.Format switch
            {
                ExportFormat.Excel => new ExcelExportService(),
                ExportFormat.Pdf => new PdfExportService(),
                _ => throw new NotSupportedException("Formato de exportación no soportado.")
            };
            return exporter.Export(ventas, options);
        }
    }
}
