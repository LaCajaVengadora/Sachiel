using ClosedXML.Excel;
using Sachiel.Models;

namespace Sachiel.Services.Export
{
    public class ExcelExportService : IExportService<Venta>
    {
        public bool Export(List<Venta> ventas, ExportOptions options)
        {
            try
            {
                using var workbook = new XLWorkbook();

                // ---------- HOJA VENTAS ----------
                var ws = workbook.Worksheets.Add("Ventas");

                // ---------- RESUMEN ----------
                ws.Cell(1, 1).Value = "Exportado por"; ws.Cell(1, 2).Value = "Sachiel";
                ws.Cell(2, 1).Value = "Fecha"; ws.Cell(2, 2).Value = DateTime.Now;
                ws.Cell(3, 1).Value = "Período"; 
                ws.Cell(3, 2).Value = $"{options.From:dd'/'MM'/'yyyy} - {options.To:dd'/'MM'/'yyyy}";
                ws.Cell(4, 1).Value = "Ventas"; ws.Cell(4, 2).Value = ventas.Count;
                

                // ---------- Headers ----------
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
                ws.Column(5).Style.NumberFormat.NumberFormatId = 44;
                ws.Column(6).Style.NumberFormat.NumberFormatId = 44;

                ws.Columns().AdjustToContents();
                ws.Column(5).Width += 2; ws.Column(6).Width += 6;


                // ---------- HOJA DETALLES ----------
                if (options.IncludeDetails)
                {
                    var wsDetalle = workbook.Worksheets.Add("Detalles");

                    // ---------- Headers ----------
                    row = 1;
                    wsDetalle.Cell(row, 1).Value = "Venta";
                    wsDetalle.Cell(row, 2).Value = "Fecha";
                    wsDetalle.Cell(row, 3).Value = "Local";
                    wsDetalle.Cell(row, 4).Value = "Producto";
                    wsDetalle.Cell(row, 5).Value = "Cantidad";
                    wsDetalle.Cell(row, 6).Value = "Precio Unitario";
                    wsDetalle.Cell(row, 7).Value = "Subtotal";
                    wsDetalle.Cell(row, 8).Value = "Total Productos";
                    row++;

                    // ---------- Detalles ----------
                    decimal totalProductos, subtotal;
                    foreach (Venta venta in ventas)
                    {
                        totalProductos = subtotal = 0m;
                        foreach (DetalleVenta detalle in venta.Detalles)
                        {
                            subtotal = detalle.Subtotal; totalProductos += subtotal;

                            wsDetalle.Cell(row, 1).Value = venta.Id.ToString("X4");
                            wsDetalle.Cell(row, 2).Value = venta.Fecha.ToDateTime(TimeOnly.MinValue);
                            wsDetalle.Cell(row, 3).Value = venta.Local.ToString();
                            wsDetalle.Cell(row, 4).Value = detalle.Producto.Nombre;
                            wsDetalle.Cell(row, 5).Value = detalle.Cantidad;
                            wsDetalle.Cell(row, 6).Value = detalle.PrecioUnitario;
                            wsDetalle.Cell(row, 7).Value = subtotal;
                            row++;
                        }
                        if (venta.Detalles.Count != 0)
                        {
                            var cell = wsDetalle.Cell(row - 1, 8);
                            cell.Value = totalProductos;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                        }
                    }

                    // ---------- Formato ----------
                    var headerDetalle = wsDetalle.Range(1, 1, 1, 8);
                        headerDetalle.Style.Font.Bold = true;
                        headerDetalle.Style.Fill.BackgroundColor = XLColor.LightGray;
                        headerDetalle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsDetalle.Column(2).Style.DateFormat.Format = "dd/MM/yyyy";
                    wsDetalle.Column(6).Style.NumberFormat.NumberFormatId = 44;
                    wsDetalle.Column(7).Style.NumberFormat.NumberFormatId = 44;
                    wsDetalle.Column(8).Style.NumberFormat.NumberFormatId = 44;
                    wsDetalle.Columns().AdjustToContents();
                    wsDetalle.Column(2).Width += 4; wsDetalle.Column(7).Width += 6;
                }

                workbook.SaveAs(options.OutputPath);
                return true;
            }
            catch { return false; }
        }
    }
}
