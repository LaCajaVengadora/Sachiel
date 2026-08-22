
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Sachiel.Services;

namespace Sachiel.ViewModels.Dashboard
{
    public class DashViewModel
    {
        private readonly DashboardService _dashboardService;
        public ISeries[] ChartVentasSemana { get; set; }
        public Axis[] AxisVentasSemana { get; set; }


        public DashViewModel()
        {
            _dashboardService = new DashboardService();
            List<DashVentasSemana> ventasSemana = _dashboardService.GetVentasPorSemana(5);
            ChartVentasSemana = [ new ColumnSeries<int> { Values = ventasSemana.Select(v => v.Cant).ToArray() } ];
            AxisVentasSemana = [ new Axis { Labels = ventasSemana.Select(v => v.Week).ToArray() } ];
        }
    }
}
