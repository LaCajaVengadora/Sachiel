using Sachiel.Data;
using Sachiel.Models;
using System.Globalization;

namespace Sachiel.Services
{
    public class ProductoService
    {
        // CREATE
        public bool AddProducto(Producto producto)
        {
            using var ctx = new SachielContext();
            try
            {
                ctx.Productos.Add(producto);
                return ctx.SaveChanges() > 0;
            } catch { return false; }
        }

        // READ
        public List<Producto> GetProductos()
        {
            using var ctx = new SachielContext();
            return ctx.Productos.OrderBy(p => p.Nombre).ToList();
        }

        public Producto? GetProducto(int id)
        {
            using var ctx = new SachielContext();
            return ctx.Productos.Find(id);
        }
        public Producto? GetProducto(string filtro)
        {
            using var ctx = new SachielContext();
            filtro = filtro.Trim();

            if (string.IsNullOrWhiteSpace(filtro)) return null;

            if (int.TryParse(filtro, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int id))
            {
                return ctx.Productos.FirstOrDefault(p => p.Id == id);
            }
            return ctx.Productos.OrderBy(p => p.Nombre).FirstOrDefault(p => p.Nombre.Contains(filtro));
        }

        // UPDATE
        public bool UpdateProducto(Producto producto)
        {
            using var ctx = new SachielContext();

            try
            {
                ctx.Productos.Update(producto);
                return ctx.SaveChanges() > 0;
            } catch { return false; }
        }


        // DELETE
        public bool DeleteProducto(int id)
        {
            using var ctx = new SachielContext();
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
