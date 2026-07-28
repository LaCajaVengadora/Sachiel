
namespace Sachiel.Services.Export
{
    public class ExportOptions
    {
        public ExportFormat Format { get; set; }
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
        public string OutputPath { get; set; } = "";
        public bool IncludeDetails { get; set; } = true;
    }
}
