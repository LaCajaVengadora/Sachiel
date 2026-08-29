namespace Sachiel.Services.Export
{
    public interface IExportService<T>
    {
        ExportFormat Format { get; }
        bool Export(List<T> data, ExportOptions options);
    }
}