using Microsoft.EntityFrameworkCore;
using Sachiel.Data;
using Sachiel.Models;
using Sachiel.ViewModels.Dashboard;

namespace Sachiel.Services
{
    public class DashboardService(IDbContextFactory<SachielContext> ctxFactory)
    {
        private readonly IDbContextFactory<SachielContext> _ctxFactory = ctxFactory;

        private static DateOnly GetInicioPeriodo(int weeks)
        {
            DateOnly hoy = DateOnly.FromDateTime(DateTime.Today);
            int numHoy = ((int)hoy.DayOfWeek + 6) % 7;
            DateOnly esteLunes = hoy.AddDays(-numHoy);
            DateOnly inicioPeriodo = esteLunes.AddDays(-(weeks - 1) * 7); //e.g.: weeks = 5 => lunes - 28 días
            return inicioPeriodo;
        }

        public List<DashVentasSemana> GetVentasPorSemana(int weeks)
        {
            try
            {
                using var ctx = _ctxFactory.CreateDbContext();
                List<DashVentasSemana> result = [];

                DateOnly inicioPeriodo = GetInicioPeriodo(weeks);

                var ventasGroup = ctx.Ventas.Where(v => v.Fecha >= inicioPeriodo)
                    .GroupBy(v => (v.Fecha.DayNumber - inicioPeriodo.DayNumber) / 7)
                    .Select(g => new {Semana = g.Key, Cant = g.Count()})
                    .ToList();

                for (int i = 0; i < weeks; i++)
                {
                    DateOnly inicioSemana = inicioPeriodo.AddDays(i * 7);
                    int cant = ventasGroup.FirstOrDefault(v => v.Semana == i)?.Cant ?? 0;
                    result.Add(new DashVentasSemana { Week = $"{inicioSemana:MM/dd}", Cant = cant});
                }

                return result;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message, ex); return []; }
        }


        public List<DashVentasPorLocal> GetVentasPorLocal(int weeks)
        {
            try
            {
                using var ctx = _ctxFactory.CreateDbContext();
                List<DashVentasPorLocal> result = [];

                var ventasGroup = ctx.Ventas.Where(v => v.Fecha >= GetInicioPeriodo(weeks))
                    .GroupBy(v => v.Local)
                    .Select(g => new {Local = g.Key, Cant = g.Count()})
                    .ToList();
                int totalVentas = ventasGroup.Sum(v => v.Cant);

                foreach (Local local in Enum.GetValues<Local>())
                {
                    int cant = ventasGroup.FirstOrDefault(v => v.Local == local)?.Cant ?? 0;
                    double porcentaje = totalVentas == 0 ? 0 : (double)cant / totalVentas * 100;
                    result.Add(new DashVentasPorLocal { Local = local, Cant = cant, Porcentaje = porcentaje });
                }

                return result;
            } 
            catch (Exception ex) { Console.WriteLine(ex.Message, ex); return []; }
        }

        public List<DashProductoVendido> GetTopProductos(int cantProductos)
        {
            try
            {
                using var ctx = _ctxFactory.CreateDbContext();
                DateOnly hoy = DateOnly.FromDateTime(DateTime.Today);
                DateOnly inicioPeriodo = hoy.AddDays(-30);

                return ctx.DetallesVenta
                    .Where(d => d.Venta.Fecha >= inicioPeriodo)
                    .GroupBy(d => d.Producto.Nombre)
                    .Select(g => new DashProductoVendido { Nombre = g.Key, Cant = g.Sum(d => d.Cantidad)})
                    .OrderByDescending(p => p.Cant)
                    .Take(cantProductos)
                    .ToList();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return []; }
        }

    }
}
