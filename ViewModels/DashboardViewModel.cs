using LENTERA.WPF.Models;
using LENTERA.WPF.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace LENTERA.WPF.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IBookingRepository _repository;

        private int _totalLabAktif;
        public int TotalLabAktif
        {
            get => _totalLabAktif;
            set { _totalLabAktif = value; OnPropertyChanged(); }
        }

        private int _bookingHariIni;
        public int BookingHariIni
        {
            get => _bookingHariIni;
            set { _bookingHariIni = value; OnPropertyChanged(); }
        }

        private int _menungguPersetujuan;
        public int MenungguPersetujuan
        {
            get => _menungguPersetujuan;
            set { _menungguPersetujuan = value; OnPropertyChanged(); }
        }

        private int _totalPengguna;
        public int TotalPengguna
        {
            get => _totalPengguna;
            set { _totalPengguna = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LabUsageStat> GrafikPenggunaan { get; } = new();

        public DashboardViewModel() : this(new BookingRepository())
        {
        }

        public DashboardViewModel(IBookingRepository repository)
        {
            _repository = repository;
            Refresh();
        }

        /// <summary>Muat ulang seluruh statistik dari database. Dipanggil setiap kali halaman Dashboard dibuka.</summary>
        public void Refresh()
        {
            try
            {
                var stats = _repository.GetDashboardStats();
                TotalLabAktif = stats.totalLab;
                BookingHariIni = stats.bookingHariIni;
                MenungguPersetujuan = stats.pendingApproval;
                TotalPengguna = stats.totalPengguna;

                GrafikPenggunaan.Clear();
                foreach (var item in _repository.GetGrafikPenggunaan())
                {
                    GrafikPenggunaan.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data dashboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
