using Sachiel.Models;

namespace Sachiel.ViewModels.Dashboard
{
    public class DashVentasPorLocal
    {
        public Local Local {  get; set; }
        public int Cant {  get; set; } // cantidad de ventas de UN local
        public double Porcentaje { get; set; }
    }
}
