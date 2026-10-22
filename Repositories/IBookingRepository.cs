using LENTERA.WPF.Models;
using System;
using System.Collections.Generic;

namespace LENTERA.WPF.Repositories
{
    public interface IBookingRepository
    {
        List<Booking> GetAllBookings();
        List<Booking> SearchBookings(string searchQuery);
        bool CekBentrokJadwal(string namaLab, DateTime tanggal, TimeSpan jamMulai, TimeSpan jamSelesai);
        bool TambahBooking(Booking booking);
        bool UpdateStatusBooking(int bookingId, string status, string alasanPenolakan);
        bool HapusBooking(int bookingId);
        List<string> GetDaftarLaboratorium();
        (int totalLab, int bookingHariIni, int pendingApproval, int totalPengguna) GetDashboardStats();
        List<LabUsageStat> GetGrafikPenggunaan();
    }
}
