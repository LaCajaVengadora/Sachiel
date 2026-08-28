using Microsoft.EntityFrameworkCore;
using Sachiel.Data;
using Sachiel.Models;
using Sachiel.ViewModels;

namespace Sachiel.Services
{
    public class VentaService
    {
        private readonly IDbContextFactory<SachielContext> _ctxFactory;
        public VentaService(IDbContextFactory<SachielContext> ctxFactory) => _ctxFactory = ctxFactory;

        public bool AddVenta(Venta venta, IEnumerable<ProductoVenta> productosVenta)
        {
            foreach (var p in productosVenta) venta.Detalles.Add(new DetalleVenta {
                ProductoId = p.Producto.Id,
                Cantidad = p.Cantidad,
                PrecioUnitario = p.Producto.Precio
            });

            try
            {
                using var ctx = _ctxFactory.CreateDbContext();
                ctx.Ventas.Add(venta);
                return ctx.SaveChanges() > 0;
            }
            catch { return false; }
        }

        // TO DO MODIFICAR
        public List<Venta> GetVentasFilter(Filter filter)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (!filter.To.HasValue) filter.To = today;
            if (!filter.From.HasValue) filter.From = filter.To.Value.AddDays(-15);
            if (filter.From > filter.To) return [];

            using var ctx = _ctxFactory.CreateDbContext();
            var query = ctx.Ventas.Include(v => v.Detalles).ThenInclude(d => d.Producto).AsQueryable();

            query = query.Where(v => filter.From.Value <= v.Fecha && v.Fecha <= filter.To.Value);
            if (filter.Locales.Count != 0) query = query.Where(v => filter.Locales.Contains(v.Local));
            if (filter.Metodos.Count != 0) query = query.Where(v => filter.Metodos.Contains(v.MetodoPago));
            if (filter.Facturada.HasValue) query = query.Where(v => v.Facturada == filter.Facturada.Value);

            return query.OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id).ToList();

        }

        public List<Venta> GetVentas() 
        {
            using var ctx = _ctxFactory.CreateDbContext();
            return ctx.Ventas.Include(v => v.Detalles).ThenInclude(d => d.Producto).ToList();
        }

        public Venta? GetVenta(int id)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            return ctx.Ventas.Include(v => v.Detalles).ThenInclude(d => d.Producto).FirstOrDefault(v => v.Id == id);
        }

        public bool UpdateVenta(Venta venta)
        {
            try
            {
                using var ctx = _ctxFactory.CreateDbContext();
                ctx.Ventas.Update(venta);
                return ctx.SaveChanges() > 0;
            }
            catch { return false; }
        }
        public bool UpdateVentaFacturada(int id, bool facturada)
        {
            try
            {
                using var ctx = _ctxFactory.CreateDbContext();
                Venta? venta = ctx.Ventas.Find(id);
                if (venta == null) return false;
                venta.Facturada = facturada;
                return ctx.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public bool DeleteVenta(int id)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var venta = ctx.Ventas.Find(id);
            if (venta == null) return false;
            try
            {
                ctx.Ventas.Remove(venta);
                return ctx.SaveChanges() > 0;
            }
            catch { return false; }
        }
    }
}

