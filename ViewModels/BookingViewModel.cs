using LENTERA.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LENTERA.WPF.ViewModels
{
    public class BookingViewModel : BaseViewModel
    {
        private ObservableCollection<Booking> _allBookings;
        private ObservableCollection<Booking> _filteredBookings;
        private Booking _newBooking = new();
        private string _searchQuery = string.Empty;

        public ObservableCollection<Booking> FilteredBookings
        {
            get => _filteredBookings;
            set { _filteredBookings = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> DaftarLaboratorium { get; set; }

        public Booking NewBooking
        {
            get => _newBooking;
            set { _newBooking = value; OnPropertyChanged(); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value ?? string.Empty;
                OnPropertyChanged();
                ApplySearch();
            }
        }

        public ICommand SimpanBookingCommand { get; }

        public BookingViewModel()
        {
            DaftarLaboratorium = new ObservableCollection<string>
            {
                "Lab Pemrograman",
                "Lab Jaringan Komputer",
                "Lab Multimedia",
                "Lab Sistem Informasi",
                "Lab Kecerdasan Buatan",
                "Lab Basis Data"
            };

            _allBookings = new ObservableCollection<Booking>
            {
                new Booking { BookingId = 1, KodeBooking = "BK-001", NamaPeminjam = "Mahirah Yasmin", NamaLab = "Lab Pemrograman", Tanggal = DateTime.Today, JamMulai = new TimeSpan(8,0,0), JamSelesai = new TimeSpan(10,0,0), Keperluan = "Praktikum Pemrograman", Status = "Disetujui" },
                new Booking { BookingId = 2, KodeBooking = "BK-002", NamaPeminjam = "Budi Santoso", NamaLab = "Lab Jaringan Komputer", Tanggal = DateTime.Today, JamMulai = new TimeSpan(10,0,0), JamSelesai = new TimeSpan(12,0,0), Keperluan = "Ujian Sertifikasi", Status = "Menunggu" }
            };

            _filteredBookings = new ObservableCollection<Booking>(_allBookings);
            SimpanBookingCommand = new RelayCommand(_ => BuatBooking());
        }

        private void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredBookings = new ObservableCollection<Booking>(_allBookings);
            }
            else
            {
                var q = SearchQuery.Trim().ToLower();
                var res = _allBookings.Where(b => (b.KodeBooking != null && b.KodeBooking.ToLower().Contains(q)) || 
                                                 (b.NamaPeminjam != null && b.NamaPeminjam.ToLower().Contains(q)) || 
                                                 (b.NamaLab != null && b.NamaLab.ToLower().Contains(q)));
                FilteredBookings = new ObservableCollection<Booking>(res);
            }
        }

        private void BuatBooking()
        {
            if (string.IsNullOrWhiteSpace(NewBooking.NamaPeminjam))
            {
                MessageBox.Show("Nama peminjam wajib diisi!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewBooking.NamaLab))
            {
                MessageBox.Show("Silakan pilih laboratorium!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewBooking.JumlahPeserta <= 0)
            {
                MessageBox.Show("Jumlah peserta minimal 1 orang!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isBentrok = _allBookings.Any(b =>
                b.NamaLab.Equals(NewBooking.NamaLab, StringComparison.OrdinalIgnoreCase) &&
                b.Tanggal.Date == NewBooking.Tanggal.Date &&
                b.Status == "Disetujui" &&
                (NewBooking.JamMulai < b.JamSelesai && NewBooking.JamSelesai > b.JamMulai));

            if (isBentrok)
            {
                MessageBox.Show("Jadwal bentrok dengan reservasi lain yang telah Disetujui!", 
                                "Deteksi Bentrok Jadwal", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int newId = _allBookings.Count + 1;
            Booking bookingToAdd = new Booking
            {
                BookingId = newId,
                KodeBooking = $"BK-{newId:D3}",
                NamaPeminjam = NewBooking.NamaPeminjam.Trim(),
                NamaLab = NewBooking.NamaLab,
                Tanggal = NewBooking.Tanggal,
                JamMulai = NewBooking.JamMulai,
                JamSelesai = NewBooking.JamSelesai,
                Keperluan = string.IsNullOrWhiteSpace(NewBooking.Keperluan) ? "Kegiatan Laboratorium" : NewBooking.Keperluan.Trim(),
                JumlahPeserta = NewBooking.JumlahPeserta,
                Status = "Menunggu"
            };

            _allBookings.Add(bookingToAdd);
            ApplySearch();

            MessageBox.Show($"Pemesanan berhasil dibuat dengan kode {bookingToAdd.KodeBooking}!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
            NewBooking = new Booking();
        }
    }
}