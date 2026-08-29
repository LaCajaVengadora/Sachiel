using Microsoft.EntityFrameworkCore;
using Sachiel.Data;
using Sachiel.Models;
using System.Globalization;

namespace Sachiel.Services
{
    public class ProductoService(IDbContextFactory<SachielContext> ctxFactory)
    {
        private readonly IDbContextFactory<SachielContext> _ctxFactory = ctxFactory;
        public bool AddProducto(Producto producto)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            try
            {
                if (ctx.Productos.FirstOrDefault(p => p.Nombre == producto.Nombre) is not null)
                    return false;
                ctx.Productos.Add(producto);
                return ctx.SaveChanges() > 0;
            } catch { return false; }
        }

        public List<Producto> GetProductos()
        {
            using var ctx = _ctxFactory.CreateDbContext();
            return ctx.Productos.OrderBy(p => p.Nombre).ToList();
        }

        public Producto? GetProducto(int id)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            return ctx.Productos.Find(id);
        }
        // TODO: modificar para unificar el case-insensitive?
        public Producto? GetProducto(string filtro) 
        {
            using var ctx = _ctxFactory.CreateDbContext();
            filtro = filtro.Trim();

            if (string.IsNullOrWhiteSpace(filtro)) return null;

            if (int.TryParse(filtro, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int id))
            {
                return ctx.Productos.FirstOrDefault(p => p.Id == id);
            }
            return ctx.Productos.OrderBy(p => p.Nombre).FirstOrDefault(p => p.Nombre.Contains(filtro));
        }

        public bool UpdateProducto(Producto producto)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            try
            {
                ctx.Productos.Update(producto);
                return ctx.SaveChanges() > 0;
            } catch { return false; }
        }

        public bool DeleteProducto(int id)
        {
            using var ctx = _ctxFactory.CreateDbContext();
            var producto = ctx.Productos.Find(id);

            if (producto == null) return false;

            try
            {
                ctx.Productos.Remove(producto);
                return ctx.SaveChanges() > 0;
            } catch { return false; }
        }
    }
}
