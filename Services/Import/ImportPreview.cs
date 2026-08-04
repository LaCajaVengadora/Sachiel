using Sachiel.Models;

namespace Sachiel.Services.Import
{
    public class ImportPreview
    {
        public List<Producto> Nuevos { get; set; } = [];
        public List<UpdatedProduct> Actualizados { get; set; } = [];
        public int SinCambios { get; set; }
    }
}
