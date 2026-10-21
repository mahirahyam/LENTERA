using System;
using System.Windows;
using System.Windows.Input;

namespace LENTERA.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentView;
        private bool _isDarkMode = false;

        public BaseViewModel CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand NavigateDashboardCommand { get; }
        public ICommand NavigateBookingCommand { get; }
        public ICommand ToggleThemeCommand { get; }

        public MainViewModel()
        {
            _currentView = new DashboardViewModel();

            NavigateDashboardCommand = new RelayCommand(_ => CurrentView = new DashboardViewModel());
            NavigateBookingCommand = new RelayCommand(_ => CurrentView = new BookingViewModel());
            ToggleThemeCommand = new RelayCommand(_ => SwitchTheme());
        }

        private void SwitchTheme()
        {
            _isDarkMode = !_isDarkMode;

            string themePath = _isDarkMode 
                ? "pack://application:,,,/Themes/DarkTheme.xaml" 
                : "pack://application:,,,/Themes/LightTheme.xaml";

            try
            {
                var themeUri = new Uri(themePath, UriKind.Absolute);
                ResourceDictionary resourceDict = new ResourceDictionary { Source = themeUri };

                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal mengganti tema: {ex.Message}", "Error Tema", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}