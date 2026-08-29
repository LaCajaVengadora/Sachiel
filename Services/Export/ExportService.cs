using Sachiel.Models;

namespace Sachiel.Services.Export
{
    public class ExportService(IEnumerable<IExportService<Venta>> exporters)
    {
        private readonly IEnumerable<IExportService<Venta>> _exporters = exporters;

        public bool Export(List<Venta> ventas, ExportOptions options)
        {
            IExportService<Venta>? exporter = _exporters.FirstOrDefault(e => e.Format == options.Format);
            return exporter == null ?
                throw new NotSupportedException("Formato de exportación no soportado.") :
                exporter.Export(ventas, options);
        }
    }
}
