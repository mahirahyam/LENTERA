using System;
using System.Windows;
using System.Windows.Input;

namespace LENTERA.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Dibuat sekali saja dan dipakai ulang, supaya data (misalnya reservasi yang baru
        // dibuat atau teks pencarian) tidak hilang setiap kali pindah halaman.
        private readonly DashboardViewModel _dashboardViewModel = new();
        private readonly BookingViewModel _bookingViewModel = new();
        private readonly ReservationViewModel _reservationViewModel = new();

        private BaseViewModel _currentView;
        private bool _isDarkMode = false;

        public BaseViewModel CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand NavigateDashboardCommand { get; }
        public ICommand NavigateBookingCommand { get; }
        public ICommand NavigateReservationCommand { get; }
        public ICommand ToggleThemeCommand { get; }

        public MainViewModel()
        {
            _currentView = _dashboardViewModel;

            NavigateDashboardCommand = new RelayCommand(_ => NavigateToDashboard());
            NavigateBookingCommand = new RelayCommand(_ => NavigateToBooking());
            NavigateReservationCommand = new RelayCommand(_ => NavigateToReservation());
            ToggleThemeCommand = new RelayCommand(_ => SwitchTheme());
        }

        private void NavigateToDashboard()
        {
            _dashboardViewModel.Refresh();
            CurrentView = _dashboardViewModel;
        }

        private void NavigateToBooking()
        {
            _bookingViewModel.Refresh();
            CurrentView = _bookingViewModel;
        }

        private void NavigateToReservation()
        {
            _reservationViewModel.Refresh();
            CurrentView = _reservationViewModel;
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
