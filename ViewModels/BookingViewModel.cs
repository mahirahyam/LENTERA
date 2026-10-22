using LENTERA.WPF.Models;
using LENTERA.WPF.Repositories;
using LENTERA.WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace LENTERA.WPF.ViewModels
{
    public class BookingViewModel : BaseViewModel
    {
        private readonly IBookingRepository _repository;

        private ObservableCollection<Booking> _filteredBookings = new();
        private Booking _newBooking = new();
        private string _searchQuery = string.Empty;
        private Booking? _selectedBooking;
        private DateTime? _tanggalBooking = DateTime.Today;
        private string _jamMulaiTerpilih = "08:00";
        private string _jamSelesaiTerpilih = "10:00";

        public ObservableCollection<Booking> FilteredBookings
        {
            get => _filteredBookings;
            set { _filteredBookings = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> DaftarLaboratorium { get; } = new();

        /// <summary>Pilihan jam yang bisa dipilih di form (07:00 - 20:30, tiap 30 menit).</summary>
        public ObservableCollection<string> JamSlots { get; } = new();

        public Booking NewBooking
        {
            get => _newBooking;
            set { _newBooking = value; OnPropertyChanged(); }
        }

        public Booking? SelectedBooking
        {
            get => _selectedBooking;
            set
            {
                _selectedBooking = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanApprove));
                OnPropertyChanged(nameof(CanReject));
                OnPropertyChanged(nameof(CanHapus));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public DateTime? TanggalBooking
        {
            get => _tanggalBooking;
            set { _tanggalBooking = value; OnPropertyChanged(); }
        }

        public string JamMulaiTerpilih
        {
            get => _jamMulaiTerpilih;
            set { _jamMulaiTerpilih = value; OnPropertyChanged(); }
        }

        public string JamSelesaiTerpilih
        {
            get => _jamSelesaiTerpilih;
            set { _jamSelesaiTerpilih = value; OnPropertyChanged(); }
        }

        public bool CanApprove => SelectedBooking != null && SelectedBooking.Status == "Menunggu";
        public bool CanReject => SelectedBooking != null && SelectedBooking.Status == "Menunggu";
        public bool CanHapus => SelectedBooking != null && SelectedBooking.Status != "Disetujui";

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
        public ICommand SetujuiBookingCommand { get; }
        public ICommand TolakBookingCommand { get; }
        public ICommand HapusBookingCommand { get; }
        public ICommand RefreshCommand { get; }

        public BookingViewModel() : this(new BookingRepository())
        {
        }

        public BookingViewModel(IBookingRepository repository)
        {
            _repository = repository;

            for (int jam = 7; jam <= 20; jam++)
            {
                JamSlots.Add($"{jam:D2}:00");
                JamSlots.Add($"{jam:D2}:30");
            }

            SimpanBookingCommand = new RelayCommand(_ => BuatBooking());
            SetujuiBookingCommand = new RelayCommand(_ => SetujuiBooking(), _ => CanApprove);
            TolakBookingCommand = new RelayCommand(_ => TolakBooking(), _ => CanReject);
            HapusBookingCommand = new RelayCommand(_ => HapusBooking(), _ => CanHapus);
            RefreshCommand = new RelayCommand(_ => Refresh());

            Refresh();
        }

        /// <summary>Muat ulang daftar lab & daftar booking dari database. Dipanggil setiap kali halaman ini dibuka.</summary>
        public void Refresh()
        {
            try
            {
                string? labTerpilih = NewBooking.NamaLab;

                DaftarLaboratorium.Clear();
                foreach (var lab in _repository.GetDaftarLaboratorium())
                {
                    DaftarLaboratorium.Add(lab);
                }

                if (DaftarLaboratorium.Count == 0)
                {
                    // Fallback jika tabel Laboratorium kosong, supaya form tetap bisa dipakai.
                    foreach (var lab in new[] { "Lab Pemrograman", "Lab Jaringan Komputer", "Lab Multimedia", "Lab Sistem Informasi" })
                        DaftarLaboratorium.Add(lab);
                }

                if (!string.IsNullOrEmpty(labTerpilih) && DaftarLaboratorium.Contains(labTerpilih))
                {
                    NewBooking.NamaLab = labTerpilih;
                }

                ApplySearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySearch()
        {
            try
            {
                var hasil = string.IsNullOrWhiteSpace(SearchQuery)
                    ? _repository.GetAllBookings()
                    : _repository.SearchBookings(SearchQuery.Trim());

                FilteredBookings = new ObservableCollection<Booking>(hasil);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal mencari data booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (!TanggalBooking.HasValue)
            {
                MessageBox.Show("Silakan pilih tanggal peminjaman!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TanggalBooking.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Tanggal peminjaman tidak boleh tanggal yang sudah lewat!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(JamMulaiTerpilih, out var jamMulai) || !TimeSpan.TryParse(JamSelesaiTerpilih, out var jamSelesai))
            {
                MessageBox.Show("Jam mulai/selesai tidak valid!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (jamSelesai <= jamMulai)
            {
                MessageBox.Show("Jam selesai harus lebih besar dari jam mulai!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewBooking.JumlahPeserta <= 0)
            {
                MessageBox.Show("Jumlah peserta minimal 1 orang!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool isBentrok = _repository.CekBentrokJadwal(NewBooking.NamaLab, TanggalBooking.Value, jamMulai, jamSelesai);
                if (isBentrok)
                {
                    MessageBox.Show("Jadwal bentrok dengan reservasi lain yang telah Disetujui!",
                                    "Deteksi Bentrok Jadwal", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var bookingBaru = new Booking
                {
                    NamaPeminjam = NewBooking.NamaPeminjam.Trim(),
                    NamaLab = NewBooking.NamaLab,
                    Tanggal = TanggalBooking.Value.Date,
                    JamMulai = jamMulai,
                    JamSelesai = jamSelesai,
                    Keperluan = string.IsNullOrWhiteSpace(NewBooking.Keperluan) ? "Kegiatan Laboratorium" : NewBooking.Keperluan.Trim(),
                    JumlahPeserta = NewBooking.JumlahPeserta
                };

                _repository.TambahBooking(bookingBaru);

                MessageBox.Show($"Pemesanan berhasil dibuat dengan kode {bookingBaru.KodeBooking}! Status: Menunggu persetujuan.",
                                "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

                string labSebelumnya = NewBooking.NamaLab;
                NewBooking = new Booking { NamaLab = labSebelumnya };
                TanggalBooking = DateTime.Today;
                JamMulaiTerpilih = "08:00";
                JamSelesaiTerpilih = "10:00";

                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menyimpan booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetujuiBooking()
        {
            if (SelectedBooking == null) return;

            var konfirmasi = MessageBox.Show(
                $"Setujui reservasi {SelectedBooking.KodeBooking} atas nama {SelectedBooking.NamaPeminjam}?",
                "Konfirmasi Persetujuan", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (konfirmasi != MessageBoxResult.Yes) return;

            try
            {
                bool isBentrok = _repository.CekBentrokJadwal(
                    SelectedBooking.NamaLab, SelectedBooking.Tanggal, SelectedBooking.JamMulai, SelectedBooking.JamSelesai);

                if (isBentrok)
                {
                    MessageBox.Show("Tidak bisa menyetujui: jadwal ini bentrok dengan reservasi lain yang sudah Disetujui!",
                                    "Deteksi Bentrok Jadwal", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _repository.UpdateStatusBooking(SelectedBooking.BookingId, "Disetujui", string.Empty);
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menyetujui booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TolakBooking()
        {
            if (SelectedBooking == null) return;

            var dialog = new InputDialog("Alasan Penolakan", $"Masukkan alasan penolakan untuk {SelectedBooking.KodeBooking}:")
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                _repository.UpdateStatusBooking(SelectedBooking.BookingId, "Ditolak", dialog.InputText.Trim());
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menolak booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HapusBooking()
        {
            if (SelectedBooking == null) return;

            var konfirmasi = MessageBox.Show(
                $"Hapus reservasi {SelectedBooking.KodeBooking}? Tindakan ini tidak dapat dibatalkan.",
                "Konfirmasi Hapus", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (konfirmasi != MessageBoxResult.Yes) return;

            try
            {
                _repository.HapusBooking(SelectedBooking.BookingId);
                SelectedBooking = null;
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menghapus booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
