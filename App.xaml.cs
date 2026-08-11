using Sachiel.Services;
using System.Windows;

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
        }
    }

}
