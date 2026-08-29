namespace Sachiel.Services.Import
{
    public class UpdatedProduct
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioAnterior { get; set; }
        public decimal PrecioNuevo { get; set; }
    }
}
