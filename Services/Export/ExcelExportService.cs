using Sachiel.Models;
using ClosedXML.Excel;

namespace Sachiel.Services.Export
{
    public class ExcelExportService : IExportService<Venta>
    {
        public bool Export(List<Venta> ventas, ExportOptions options)
        {
            try
            {
                using var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Ventas");

                // ---------- RESUMEN ----------
                ws.Cell(1, 1).Value = "Exportado por"; ws.Cell(1, 2).Value = "Sachiel";
                ws.Cell(2, 1).Value = "Fecha"; ws.Cell(2, 2).Value = DateTime.Now;
                ws.Cell(3, 1).Value = "Período"; 
                ws.Cell(3, 2).Value =
    $"{options.From.ToString("dd/MM/yyyy")} - {options.To.ToString("dd/MM/yyyy")}";
                ws.Cell(4, 1).Value = "Ventas"; ws.Cell(4, 2).Value = ventas.Count;
                

                // ---------- Encabezados ----------
                int row = 7;
                ws.Cell(row, 1).Value = "Código";
                ws.Cell(row, 2).Value = "Fecha";
                ws.Cell(row, 3).Value = "Local";
                ws.Cell(row, 4).Value = "Método";
                ws.Cell(row, 5).Value = "Descuento";
                ws.Cell(row, 6).Value = "Total";
                ws.Cell(row, 7).Value = "Facturada";
                row++;

                // ---------- Ventas ----------
                foreach (Venta venta in ventas)
                {
                    ws.Cell(row, 1).Value = venta.Id.ToString("X4");
                    ws.Cell(row, 2).Value = venta.Fecha.ToDateTime(TimeOnly.MinValue);
                    ws.Cell(row, 3).Value = venta.Local.ToString();
                    ws.Cell(row, 4).Value = venta.MetodoPago.ToString();
                    ws.Cell(row, 5).Value = venta.Descuento;
                    ws.Cell(row, 6).Value = venta.PrecioTotal;
                    ws.Cell(row, 7).Value = venta.Facturada;
                    row++;
                }

                // ---------- Formato ----------
                ws.Range(1, 1, 5, 1).Style.Font.Bold = true;
                var header = ws.Range(7, 1, 7, 7);
                    header.Style.Font.Bold = true;
                    header.Style.Fill.BackgroundColor = XLColor.LightGray;
                    header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Column(2).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Cell(4, 2).Style.NumberFormat.Format = "0";
                ws.Column(5).Style.NumberFormat.Format = "$#,##0.00";
                ws.Column(6).Style.NumberFormat.Format = "$#,##0.00";

                ws.Columns().AdjustToContents();
                workbook.SaveAs(options.OutputPath);

                return true;
            }
            catch { return false; }
        }
    }
}
