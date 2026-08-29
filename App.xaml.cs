using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sachiel.Data;
using Sachiel.Models;
using Sachiel.Services;
using Sachiel.Services.Export;
using Sachiel.Services.Import;
using Sachiel.ViewModels.Dashboard;
using Sachiel.Views;
using System.Windows;

namespace Sachiel
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public static AppSettingsService Settings => Services.GetRequiredService<AppSettingsService>();

        public App()
        {
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();

            Settings.Load();

            using var ctx = Services.GetRequiredService<IDbContextFactory<SachielContext>>().CreateDbContext();
            ctx.Database.Migrate();
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<SachielContext>();
            services.AddSingleton<AppSettingsService>();

            services.AddTransient<VentaService>();
            services.AddTransient<ProductoService>();
            services.AddTransient<DashboardService>();
            services.AddTransient<BackupService>();
            services.AddTransient<ImportService>();
            services.AddTransient<ExportService>();
            services.AddTransient<IExportService<Venta>, ExcelExportService>();
            services.AddTransient<IExportService<Venta>, PdfExportService>();

            services.AddTransient<DashViewModel>();

            services.AddSingleton<IViewFactory, ViewFactory>();

            services.AddSingleton<MainWindow>(); 
            
            services.AddTransient<DashboardView>(); 
            services.AddTransient<VentaView>(); 
            services.AddTransient<ProductoView>(); 
            services.AddTransient<ConfigView>(); 
            services.AddTransient<AddVentaView>(); 
            services.AddTransient<ExportVentasView>(); 
            services.AddTransient<ImportPreviewView>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = Services.GetRequiredService<MainWindow>();
            MainWindow = window;
            window.Show();
        }

    }

}
