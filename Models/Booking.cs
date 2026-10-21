using System;

namespace LENTERA.WPF.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string KodeBooking { get; set; } = string.Empty;
        public string NamaPeminjam { get; set; } = string.Empty;
        public string NamaLab { get; set; } = string.Empty;
        public DateTime Tanggal { get; set; } = DateTime.Today;
        public TimeSpan JamMulai { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan JamSelesai { get; set; } = new TimeSpan(10, 0, 0);
        public string Keperluan { get; set; } = string.Empty;
        public int JumlahPeserta { get; set; } = 1;
        public string Status { get; set; } = "Menunggu";
        public string AlasanPenolakan { get; set; } = string.Empty;
    }
}