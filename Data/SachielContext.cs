using Microsoft.EntityFrameworkCore;
using Sachiel.Models;
using System.IO;

namespace Sachiel.Data
{
    public class SachielContext : DbContext
    {
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Database");
            Directory.CreateDirectory(databaseDirectory);
            string dbPath = Path.Combine(databaseDirectory, "Sachiel.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
