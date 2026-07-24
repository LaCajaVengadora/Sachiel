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

        // READ
        public List<Venta> GetVentas()
        {
            using var ctx = new SachielContext();
            return ctx.Ventas.Include(v => v.Detalles).ThenInclude(d => d.Producto).ToList();
        }

        public Venta? GetVenta(int id)
        {
            using var ctx = new SachielContext();
            return ctx.Ventas.Find(id);
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

