using Sachiel.Models;

namespace Sachiel.ViewModels
{
    public class Filter
    {
        public List<Local> Locales { get; set; } = [];
        public DateOnly? From { get; set; } = null;
        public DateOnly? To { get; set; } = null;
        public List<Metodo> Metodos { get; set; } = [];
        public bool? Facturada { get; set; } = null;

    }
}
