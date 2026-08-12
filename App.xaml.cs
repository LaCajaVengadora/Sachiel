using Sachiel.Data;
using Sachiel.Services;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace Sachiel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AppSettingsService Settings { get; } = new();
        public App()
        {
            Settings.Load();
            using var ctx = new SachielContext();
            ctx.Database.Migrate();
        }
    }

}
