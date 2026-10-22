using LENTERA.WPF.Services;
using System.Windows;

namespace LENTERA.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Harus dibuat SEBELUM base.OnStartup(e), karena base.OnStartup akan langsung
            // membuka MainWindow (lewat StartupUri) yang membuat MainViewModel, yang langsung
            // query database lewat BookingViewModel/DashboardViewModel. Kalau urutannya terbalik,
            // query pertama akan gagal karena tabel belum sempat dibuat.
            try
            {
                DatabaseHelper.InitializeDatabase();
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Gagal menyiapkan database: {ex.Message}",
                    "Error Database", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}