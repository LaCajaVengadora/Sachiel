
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Sachiel.Models;
using Sachiel.Services;
using SkiaSharp;

namespace Sachiel.ViewModels.Dashboard
{
    public class DashViewModel
    {
        private readonly DashboardService _dashboardService;
        public ISeries[] ChartVentasSemana { get; set; }
        public Axis[] XAxisVentasSemana { get; set; } public Axis[] YAxisVentasSemana { get; set; }

        public ISeries[] PieVentasLocal { get; set; }

        public ISeries[] ChartProductos { get; set; }
        public Axis[] XAxisProductos { get; set; } public Axis[] YAxisProductos { get; set; }

        public DashViewModel()
        {
            _dashboardService = new DashboardService();

            List<DashVentasSemana> ventasSemana = _dashboardService.GetVentasPorSemana(5);
            ChartVentasSemana = [ new ColumnSeries<int> { Values = ventasSemana.Select(v => v.Cant).ToArray(),
                                                          Fill = new SolidColorPaint(SKColor.Parse("#2C5F1E")) }, ];
            XAxisVentasSemana = [ new Axis { Labels = ventasSemana.Select(v => v.Week).ToArray() } ];
            YAxisVentasSemana = [ new Axis { SeparatorsPaint = new SolidColorPaint(SKColors.Black) } ];

            List<DashVentasPorLocal> ventasLocal = _dashboardService.GetVentasPorLocal(5);
            PieVentasLocal = ventasLocal.Select(v => new PieSeries<int> { 
                Values = [v.Cant],
                Name = $"{v.Local} ({v.Porcentaje:0}%)",
                Fill = new SolidColorPaint(SKColor.Parse(v.Local == Local.Makai ? "#2C5F1E" : "#398033"))
            }).ToArray();

            List<DashProductoVendido> productos = _dashboardService.GetTopProductos(7);
            ChartProductos = [ new RowSeries<int> { Values = productos.Select(p => p.Cant).ToArray(),
                                                    Fill = new SolidColorPaint(SKColor.Parse("#398033")),
                                                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                                                    XToolTipLabelFormatter = point => $"{productos[point.Index].Nombre}",
                                                    DataLabelsSize = 14,
                                                    //DataLabelsPosition = DataLabelsPosition.End,
                                                    DataLabelsFormatter = point => productos[point.Index].Nombre}];
            YAxisProductos = [new Axis { IsVisible = false, IsInverted = true,
                                         SeparatorsPaint = null }];
            XAxisProductos = [new Axis { SeparatorsPaint = new SolidColorPaint(SKColors.Black) }];
        }
    }
}
