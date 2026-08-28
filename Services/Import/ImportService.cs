using ClosedXML.Excel;
using Sachiel.Data;
using Sachiel.Models;

namespace Sachiel.Services.Import
{
    public class ImportService
    {
        private List<Producto> ReadExcel(string path)
        {
            using var workbook = new XLWorkbook(path);
            var ws = workbook.Worksheet(1);
            if (ws.Cell(1, 1).GetString() != "Nombre producto" || ws.Cell(1, 2).GetString() != "Precio unitario")
                throw new Exception("El formato del Excel no es válido.\nAsegurese que las primeras columnas digan\n`Nombre producto` y `Precio unitario`");
            List<Producto> productos = [];

            int row = 2;
            string nombre; decimal precio;
            while (true)
            {
                nombre = ws.Cell(row, 1).GetString().Trim();
                if (string.IsNullOrWhiteSpace(nombre)) break;

                precio = 0;
                try
                {
                    precio = ws.Cell(row, 2).GetValue<decimal>();
                    if (precio <= 0) throw new Exception();
                }
                catch
                {
                    throw new Exception($"Precio inválido en la fila {row}.");
                }

                productos.Add(new Producto {Nombre = nombre, Precio = precio});
                row++;
            }

            return productos;
        }

        private void ValidateDuplicates(List<Producto> productos)
        {
            var duplicados = productos.GroupBy(p => p.Nombre.Trim(), StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicados.Count != 0)
                throw new Exception("El archivo contiene productos duplicados:\n\n" + string.Join("\n", duplicados));
        }

        private ImportPreview CompareDB(List<Producto> productosExcel)
        {
            using var ctx = new SachielContext();
            Dictionary<string, Producto> productosDB = ctx.Productos.ToDictionary(p=>p.Nombre.Trim(), StringComparer.OrdinalIgnoreCase);
            ImportPreview preview = new();

            foreach (Producto excelP in productosExcel)
            {
                if (!productosDB.TryGetValue(excelP.Nombre.Trim(), out Producto? dbP)) preview.Nuevos.Add(excelP);
                else if (dbP.Precio != excelP.Precio)
                    preview.Actualizados.Add(
                        new UpdatedProduct{ ProductoId = dbP.Id, Nombre = dbP.Nombre, PrecioAnterior = dbP.Precio, PrecioNuevo = excelP.Precio});
                else preview.SinCambios++;
            }

            return preview;
        }

        public ImportPreview PreviewImport(string excelPath)
        {
            List<Producto> productos = ReadExcel(excelPath);
            ValidateDuplicates(productos);
            return CompareDB(productos);
        }

        public bool ApplyImport(ImportPreview preview)
        {
            using var ctx = new SachielContext();
            try
            {
                Dictionary<int, Producto> productos = ctx.Productos.ToDictionary(p => p.Id);
                foreach (Producto nuevo in preview.Nuevos) ctx.Productos.Add(nuevo);
                foreach (UpdatedProduct updated in preview.Actualizados)
                    if (productos.TryGetValue(updated.ProductoId, out Producto? producto)) producto.Precio = updated.PrecioNuevo;
                return ctx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {   
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
