using Sachiel.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Sachiel.Views
{

    public partial class DashboardView : UserControl
    {
        private readonly ProductoService _productoService = new();
        private readonly VentaService _ventaService = new();
        public DashboardView() { 
            InitializeComponent();
            LoadInitialData();
        }
        private void LoadInitialData()
        {
            DateOnly week = DateOnly.FromDateTime(DateTime.Today).AddDays(-7);
            DateOnly month = week.AddDays(-23);
            var productos = _productoService.GetProductos();
            var ventasSemana = _ventaService.GetVentasFilter(from: DateOnly.FromDateTime(DateTime.Today).AddDays(-7));
            var ventasPendientes = _ventaService.GetVentasFilter(facturada: false);

            txtProductos.Text = productos.Count.ToString();
            txtVentas.Text = ventasSemana.Count.ToString();

            decimal ingresos = ventasSemana.Sum(v => v.PrecioTotal);
            txtIngresos.Text = ingresos.ToString("C");

            int pendientes = ventasPendientes.Count(v => !v.Facturada);
            txtPendientes.Text = pendientes.ToString();

            dgVentasRecientes.ItemsSource = ventasSemana.OrderByDescending(v => v.Fecha).Take(5).ToList();
        }
    }
}
