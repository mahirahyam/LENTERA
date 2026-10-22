using LENTERA.WPF.Services;
using System.Windows;

namespace LENTERA.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatabaseHelper.InitializeDatabase();
        }
    }
}