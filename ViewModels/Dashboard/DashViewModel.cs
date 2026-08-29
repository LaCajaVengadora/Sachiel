using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Sachiel.Models;
using Sachiel.Services;
using SkiaSharp;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sachiel.ViewModels.Dashboard
{
    public class DashViewModel : INotifyPropertyChanged
    {
        private readonly DashboardService _dashboardService;
        public ISeries[] ChartVentasSemana { get; private set; } = [];
        public Axis[] XAxisVentasSemana { get; private set; } = [];
        public Axis[] YAxisVentasSemana { get; private set; } = [];

        public ISeries[] PieVentasLocal { get; private set; } = [];

        public ISeries[] ChartProductos { get; private set; } = [];
        public Axis[] XAxisProductos { get; private set; } = [];
        public Axis[] YAxisProductos { get; private set; } = [];

        public DashViewModel(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            Refresh();
        }
        public void Refresh()
        {
            LoadVentasSemana();
            LoadVentasLocal();
            LoadProductos();
        }

        private void LoadVentasSemana() 
        { 
            List<DashVentasSemana> ventasSemana = _dashboardService.GetVentasPorSemana(5);
            ChartVentasSemana = [ new ColumnSeries<int> { Values = ventasSemana.Select(v => v.Cant).ToArray(),
                                                          Fill = new SolidColorPaint(SKColor.Parse("#2C5F1E")) }, ];
            XAxisVentasSemana = [ new Axis { Labels = ventasSemana.Select(v => v.Week).ToArray() } ];
            YAxisVentasSemana = [ new Axis { SeparatorsPaint = new SolidColorPaint(SKColors.Black) } ];

            OnPropertyChanged(nameof(ChartVentasSemana)); 
            OnPropertyChanged(nameof(XAxisVentasSemana)); 
            OnPropertyChanged(nameof(YAxisVentasSemana));
        }
        private void LoadVentasLocal()
        {
            List<DashVentasPorLocal> ventasLocal = _dashboardService.GetVentasPorLocal(5);
            PieVentasLocal = ventasLocal.Select(v => new PieSeries<int> { 
                Values = [v.Cant],
                Name = $"{v.Local} ({v.Porcentaje:0}%)",
                Fill = new SolidColorPaint(SKColor.Parse(v.Local == Local.Makai ? "#2C5F1E" : "#398033"))
            }).ToArray();

            OnPropertyChanged(nameof(PieVentasLocal));
        }
        private void LoadProductos()
        {
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

            OnPropertyChanged(nameof(ChartProductos));
            OnPropertyChanged(nameof(XAxisProductos));
            OnPropertyChanged(nameof(YAxisProductos));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
    }
}
