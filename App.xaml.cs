using System;
using System.Windows;

namespace LENTERA.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Tangkap semua error tersembunyi yang bikin aplikasi mati diam-diam
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                MessageBox.Show($"CRASH TERDETEKSI:\n\n{ex?.Message}\n\nDetail:\n{ex?.InnerException?.Message}", 
                                "Error LENTERA", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            base.OnStartup(e);
        }
    }
}