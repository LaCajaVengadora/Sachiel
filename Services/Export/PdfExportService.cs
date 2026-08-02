using QuestPDF.Fluent;
using QuestPDF.Helpers;
using static QuestPDF.Helpers.Colors;
using QuestPDF.Infrastructure;
using Sachiel.Models;

namespace Sachiel.Services.Export
{
    public class PdfExportService : IExportService<Venta>
    {
        public PdfExportService() => QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community; 
        public bool Export(List<Venta> ventas, ExportOptions options)
        {

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header().PaddingBottom(20).Text("Reporte de ventas").FontSize(22).Bold();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Exportado: {DateTime.Now:dd'/'MM'/'yyyy}");
                        col.Item().Text($"Período: {options.From:dd'/'MM'/'yyyy} - {options.To:dd'/'MM'/'yyyy}");
                        col.Item().Text($"Ventas: {ventas.Count}");

                        col.Item().PaddingTop(20);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);   // Código
                                columns.ConstantColumn(80);   // Fecha
                                columns.RelativeColumn();     // Local
                                columns.RelativeColumn();     // Método
                                columns.RelativeColumn();   // Descuento
                                columns.ConstantColumn(80);   // Total
                                columns.ConstantColumn(40);   // Fact.
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Código").Bold();
                                header.Cell().Element(CellStyle).Text("Fecha").Bold();
                                header.Cell().Element(CellStyle).Text("Local").Bold();
                                header.Cell().Element(CellStyle).Text("Método").Bold();
                                header.Cell().Element(CellStyle).Text("Descuento").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();
                                header.Cell().Element(CellStyle).AlignCenter().Text("Fact.").Bold();
                            });
                            foreach (var venta in ventas)
                            {
                                table.Cell().Element(Cell).Text(venta.Id.ToString("X4"));
                                table.Cell().Element(Cell).Text(venta.Fecha.ToString("dd'/'MM'/'yyyy"));
                                table.Cell().Element(Cell).Text(venta.Local.ToString());
                                table.Cell().Element(Cell).Text(venta.MetodoPago.ToString());
                                table.Cell().Element(Cell).AlignRight().Text($"${venta.Descuento:N2}");
                                table.Cell().Element(Cell).AlignRight().Text($"${venta.PrecioTotal:N2}");
                                table.Cell().Element(Cell).AlignCenter().Text(venta.Facturada ? "Sí" : "No");

                                if (options.IncludeDetails && venta.Detalles.Count != 0)
                                {
                                    table.Cell().ColumnSpan(7).PaddingBottom(8).Element(containerP =>
                                    {
                                        containerP.Background(Grey.Lighten4).Padding(8).PaddingLeft(20).Table(detail =>
                                        {
                                            detail.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(4);   // Producto
                                                columns.ConstantColumn(55);  // Cantidad
                                                columns.ConstantColumn(90);  // Precio
                                                columns.ConstantColumn(90);  // Subtotal

                                                detail.Header(header =>
                                                {
                                                    header.Cell().Element(CellStyle).Text("Producto").SemiBold();
                                                    header.Cell().Element(CellStyle).AlignCenter().Text("Cant.").FontSize(12).SemiBold();
                                                    header.Cell().Element(CellStyle).AlignRight().Text("P. Unit.").FontSize(12).SemiBold();
                                                    header.Cell().Element(CellStyle).AlignRight().Text("Subtotal").FontSize(12).SemiBold();
                                                });

                                                foreach (var d in venta.Detalles)
                                                {
                                                    detail.Cell().Element(Cell).Text(d.Producto.Nombre).FontSize(9);
                                                    detail.Cell().Element(Cell).AlignCenter().Text(d.Cantidad.ToString()).FontSize(9);
                                                    detail.Cell().Element(Cell).AlignRight().Text($"${d.PrecioUnitario:N2}").FontSize(9);
                                                    detail.Cell().Element(Cell).AlignRight().Text($"${d.Subtotal:N2}").FontSize(9);
                                                }
                                            });
                                        });
                                    });
                                }
                            }
                        });
                    });
                });
            })
            .GeneratePdf(options.OutputPath);

            return true;
        }
        private static IContainer CellStyle(IContainer container)
        {
            return container.Background(Grey.Lighten2).Border(1).BorderColor(Grey.Lighten1).PaddingVertical(5).PaddingHorizontal(4);
        }

        private static IContainer Cell(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4);
        }
    }
}
