using System;
using Microsoft.EntityFrameworkCore;
using Sachiel.Models;

namespace Sachiel.Data
{
    public class SachielContext : DbContext
    {
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=PC-L25;" +
                "Database=Sachiel;" +
                "User Id=sa;" +
                "Password=Alteradosxpi;" +
                "TrustServerCertificate=True;");
        }
    }
}
