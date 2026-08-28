namespace Sachiel.Services.Export
{
    public interface IExportService<T>
    {
        bool Export(List<T> data, ExportOptions options);
    }
}