using Sachiel.Models;

namespace Sachiel.Services.Export
{
    public class PdfExportService : IExportService<Venta>
    {
        public bool Export(List<Venta> ventas, ExportOptions options)
        {
            // TODO
            return true;
        }
    }
}
