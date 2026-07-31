using Microsoft.EntityFrameworkCore;
using Sachiel.Data;
using Sachiel.Models;
using Sachiel.ViewModels;

namespace Sachiel.Services
{
    public class VentaService
    {
        // CREATE
        public bool AddVenta(Venta venta, IEnumerable<ProductoVenta> productosVenta)
        {
            using var ctx = new SachielContext();

            try
            {
                foreach (var p in productosVenta) venta.Detalles.Add(new DetalleVenta
                {
                    ProductoId = p.Producto.Id,
                    Cantidad = p.Cantidad,
                    PrecioUnitario = p.Producto.Precio
                });
            } catch { return false; }

            try
            {
                ctx.Ventas.Add(venta);
                return ctx.SaveChanges() > 0;
            }
            catch { return false; }
        }


        public List<Venta> GetVentasFilterACTUAL(Filter filter)
        {
            return GetVentasFilter(filter.From, filter.To, filter.Locales, filter.Metodos, filter.Facturada);
        }

        // READ
        public List<Venta> GetVentasFilter(DateOnly? from = null, DateOnly? to = null, 
            List<Local>? locales = null, List<Metodo>? metodos = null, bool? facturada = null)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (to == null) to = today;
            if (from == null) from = to.Value.AddDays(-15);
            if (from > to) return [];

            using var ctx = new SachielContext();
            var query = ctx.Ventas.Include(v => v.Detalles).ThenInclude(d => d.Producto).AsQueryable();

            if (from.HasValue) query = query.Where(v => v.Fecha >= from.Value);
            if (to.HasValue) query = query.Where(v => v.Fecha <= to.Value);
            if (locales != null && locales.Count != 0) query = query.Where(v => locales.Contains(v.Local));
            if (metodos != null && metodos.Count != 0) query = query.Where(v => metodos.Contains(v.MetodoPago));
            if (facturada.HasValue) query = query.Where(v => v.Facturada == facturada.Value);

            return query.OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id).ToList();
        }
        public List<Venta> GetVentas()
        {
            using var ctx = new SachielContext();
            return ctx.Ventas.Include(v => v.Detalles).ThenInclude(d => d.Producto).ToList();
        }
        public Venta? GetVenta(int id)
        {
            using var ctx = new SachielContext();
            return ctx.Ventas.Include(v => v.Detalles).ThenInclude(d => d.Producto).FirstOrDefault(v => v.Id == id);
        }

        // UPDATE
        public bool UpdateVenta(Venta venta)
        {
            using var ctx = new SachielContext();
            try
            {
                ctx.Ventas.Update(venta);
                return ctx.SaveChanges() > 0;
            }
            catch { return false; }
        }
        public bool UpdateVentaFacturada(int id, bool facturada)
        {
            using var ctx = new SachielContext();
            try
            {
                Venta? venta = ctx.Ventas.Find(id);
                if (venta == null) return false;
                venta.Facturada = facturada;
                return ctx.SaveChanges() > 0;
            }
            catch { return false; }
        }


        // DELETE
        public bool DeleteVenta(int id)
        {
            using var ctx = new SachielContext();
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

