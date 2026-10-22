using LENTERA.WPF.Models;
using LENTERA.WPF.Services;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace LENTERA.WPF.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        public List<Booking> GetAllBookings()
        {
            var list = new List<Booking>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = "SELECT BookingId, KodeBooking, NamaPeminjam, NamaLab, Tanggal, JamMulai, JamSelesai, Keperluan, JumlahPeserta, Status FROM Booking ORDER BY BookingId DESC";
            using var cmd = new SqliteCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Booking
                {
                    BookingId = reader.GetInt32(0),
                    KodeBooking = reader.GetString(1),
                    NamaPeminjam = reader.GetString(2),
                    NamaLab = reader.GetString(3),
                    Tanggal = DateTime.Parse(reader.GetString(4)),
                    JamMulai = TimeSpan.Parse(reader.GetString(5)),
                    JamSelesai = TimeSpan.Parse(reader.GetString(6)),
                    Keperluan = reader.GetString(7),
                    JumlahPeserta = reader.GetInt32(8),
                    Status = reader.GetString(9)
                });
            }
            return list;
        }

        public List<Booking> SearchBookings(string searchQuery)
        {
            var list = new List<Booking>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = @"SELECT BookingId, KodeBooking, NamaPeminjam, NamaLab, Tanggal, JamMulai, JamSelesai, Keperluan, JumlahPeserta, Status 
                            FROM Booking 
                            WHERE KodeBooking LIKE @q OR NamaPeminjam LIKE @q OR NamaLab LIKE @q
                            ORDER BY BookingId DESC";

            using var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddWithValue("@q", $"%{searchQuery}%");
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Booking
                {
                    BookingId = reader.GetInt32(0),
                    KodeBooking = reader.GetString(1),
                    NamaPeminjam = reader.GetString(2),
                    NamaLab = reader.GetString(3),
                    Tanggal = DateTime.Parse(reader.GetString(4)),
                    JamMulai = TimeSpan.Parse(reader.GetString(5)),
                    JamSelesai = TimeSpan.Parse(reader.GetString(6)),
                    Keperluan = reader.GetString(7),
                    JumlahPeserta = reader.GetInt32(8),
                    Status = reader.GetString(9)
                });
            }
            return list;
        }

        public bool CekBentrokJadwal(string namaLab, DateTime tanggal, TimeSpan jamMulai, TimeSpan jamSelesai)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = @"SELECT COUNT(*) FROM Booking 
                            WHERE LOWER(NamaLab) = LOWER(@namaLab) 
                            AND Tanggal = @tanggal 
                            AND Status = 'Disetujui' 
                            AND (@jamMulai < JamSelesai AND @jamSelesai > JamMulai)";

            using var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddWithValue("@namaLab", namaLab);
            cmd.Parameters.AddWithValue("@tanggal", tanggal.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@jamMulai", jamMulai.ToString(@"hh\:mm\:ss"));
            cmd.Parameters.AddWithValue("@jamSelesai", jamSelesai.ToString(@"hh\:mm\:ss"));

            long count = (long)(cmd.ExecuteScalar() ?? 0);
            return count > 0;
        }

        public bool TambahBooking(Booking booking)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = @"INSERT INTO Booking (KodeBooking, NamaPeminjam, NamaLab, Tanggal, JamMulai, JamSelesai, Keperluan, JumlahPeserta, Status) 
                            VALUES (@kode, @peminjam, @lab, @tanggal, @jamMulai, @jamSelesai, @keperluan, @peserta, 'Menunggu')";

            using var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddWithValue("@kode", booking.KodeBooking);
            cmd.Parameters.AddWithValue("@peminjam", booking.NamaPeminjam);
            cmd.Parameters.AddWithValue("@lab", booking.NamaLab);
            cmd.Parameters.AddWithValue("@tanggal", booking.Tanggal.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@jamMulai", booking.JamMulai.ToString(@"hh\:mm\:ss"));
            cmd.Parameters.AddWithValue("@jamSelesai", booking.JamSelesai.ToString(@"hh\:mm\:ss"));
            cmd.Parameters.AddWithValue("@keperluan", booking.Keperluan);
            cmd.Parameters.AddWithValue("@peserta", booking.JumlahPeserta);

            return cmd.ExecuteNonQuery() > 0;
        }

        public (int totalLab, int bookingHariIni, int pendingApproval, int totalPengguna) GetDashboardStats()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string qLab = "SELECT COUNT(*) FROM Laboratorium WHERE IsActive = 1";
            using var cmdLab = new SqliteCommand(qLab, conn);
            int totalLab = Convert.ToInt32(cmdLab.ExecuteScalar());

            string qToday = "SELECT COUNT(*) FROM Booking WHERE Tanggal = date('now')";
            using var cmdToday = new SqliteCommand(qToday, conn);
            int bookingHariIni = Convert.ToInt32(cmdToday.ExecuteScalar());

            string qPending = "SELECT COUNT(*) FROM Booking WHERE Status = 'Menunggu'";
            using var cmdPending = new SqliteCommand(qPending, conn);
            int pending = Convert.ToInt32(cmdPending.ExecuteScalar());

            return (totalLab, bookingHariIni, pending, 142);
        }

        public List<LabUsageStat> GetGrafikPenggunaan()
        {
            var list = new List<LabUsageStat>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = @"SELECT NamaLab, COUNT(*) as Total 
                            FROM Booking 
                            GROUP BY NamaLab";

            using var cmd = new SqliteCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new LabUsageStat
                {
                    NamaLab = reader.GetString(0),
                    TotalBooking = reader.GetInt32(1)
                });
            }
            return list;
        }
    }

    public interface IBookingRepository
    {
    }
}