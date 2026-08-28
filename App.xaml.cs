using Sachiel.Data;
using Sachiel.Services;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Sachiel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public static AppSettingsService Settings { get; } = new();
        // MEJORAR public static AppSettingsService Settings => Services.GetRequiredService<AppSettingsService>();
        public App()
        {
            Settings.Load();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
            using var ctx = Services.GetRequiredService<IDbContextFactory<SachielContext>>().CreateDbContext();
            ctx.Database.Migrate();
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<SachielContext>();
            //services.AddSingleton<AppSettingsService>();
            services.AddSingleton(Settings);

            services.AddTransient<VentaService>();
            services.AddTransient<ProductoService>();
            services.AddTransient<DashboardService>();
            services.AddTransient<BackupService>();
        }

    }

}
