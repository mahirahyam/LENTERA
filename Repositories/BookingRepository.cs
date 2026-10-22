using LENTERA.WPF.Models;
using LENTERA.WPF.Services;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace LENTERA.WPF.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private static Booking ReadBooking(SqliteDataReader reader)
        {
            return new Booking
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
                Status = reader.GetString(9),
                AlasanPenolakan = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
            };
        }

        private const string SelectColumns =
            "BookingId, KodeBooking, NamaPeminjam, NamaLab, Tanggal, JamMulai, JamSelesai, Keperluan, JumlahPeserta, Status, AlasanPenolakan";

        public List<Booking> GetAllBookings()
        {
            var list = new List<Booking>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = $"SELECT {SelectColumns} FROM Booking ORDER BY Tanggal DESC, JamMulai DESC, BookingId DESC";
            using var cmd = new SqliteCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(ReadBooking(reader));
            }
            return list;
        }

        public List<Booking> SearchBookings(string searchQuery)
        {
            var list = new List<Booking>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = $@"SELECT {SelectColumns}
                            FROM Booking 
                            WHERE KodeBooking LIKE @q OR NamaPeminjam LIKE @q OR NamaLab LIKE @q OR Status LIKE @q
                            ORDER BY Tanggal DESC, JamMulai DESC, BookingId DESC";

            using var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddWithValue("@q", $"%{searchQuery}%");
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(ReadBooking(reader));
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
            using var transaction = conn.BeginTransaction();

            try
            {
                // KodeBooking sementara yang pasti unik, akan diganti dengan kode final
                // begitu BookingId (autoincrement) diketahui, supaya kode booking konsisten
                // dengan ID-nya walaupun ada booking yang dihapus sebelumnya.
                string insertQuery = @"INSERT INTO Booking 
                            (KodeBooking, NamaPeminjam, NamaLab, Tanggal, JamMulai, JamSelesai, Keperluan, JumlahPeserta, Status, AlasanPenolakan) 
                            VALUES (@kode, @peminjam, @lab, @tanggal, @jamMulai, @jamSelesai, @keperluan, @peserta, 'Menunggu', '')";

                using (var cmd = new SqliteCommand(insertQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@kode", $"TMP-{Guid.NewGuid():N}");
                    cmd.Parameters.AddWithValue("@peminjam", booking.NamaPeminjam);
                    cmd.Parameters.AddWithValue("@lab", booking.NamaLab);
                    cmd.Parameters.AddWithValue("@tanggal", booking.Tanggal.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@jamMulai", booking.JamMulai.ToString(@"hh\:mm\:ss"));
                    cmd.Parameters.AddWithValue("@jamSelesai", booking.JamSelesai.ToString(@"hh\:mm\:ss"));
                    cmd.Parameters.AddWithValue("@keperluan", booking.Keperluan);
                    cmd.Parameters.AddWithValue("@peserta", booking.JumlahPeserta);
                    cmd.ExecuteNonQuery();
                }

                long newId;
                using (var idCmd = new SqliteCommand("SELECT last_insert_rowid()", conn, transaction))
                {
                    newId = (long)(idCmd.ExecuteScalar() ?? 0);
                }

                string kodeFinal = $"BK-{newId:D3}";
                using (var updateCmd = new SqliteCommand("UPDATE Booking SET KodeBooking = @kode WHERE BookingId = @id", conn, transaction))
                {
                    updateCmd.Parameters.AddWithValue("@kode", kodeFinal);
                    updateCmd.Parameters.AddWithValue("@id", newId);
                    updateCmd.ExecuteNonQuery();
                }

                transaction.Commit();

                booking.BookingId = (int)newId;
                booking.KodeBooking = kodeFinal;
                booking.Status = "Menunggu";
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public bool UpdateStatusBooking(int bookingId, string status, string alasanPenolakan)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = "UPDATE Booking SET Status = @status, AlasanPenolakan = @alasan WHERE BookingId = @id";
            using var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@alasan", alasanPenolakan ?? string.Empty);
            cmd.Parameters.AddWithValue("@id", bookingId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool HapusBooking(int bookingId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = "DELETE FROM Booking WHERE BookingId = @id";
            using var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", bookingId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public List<string> GetDaftarLaboratorium()
        {
            var list = new List<string>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = "SELECT Nama FROM Laboratorium WHERE IsActive = 1 ORDER BY Nama";
            using var cmd = new SqliteCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(reader.GetString(0));
            }
            return list;
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

            string qUsers = "SELECT COUNT(DISTINCT NamaPeminjam) FROM Booking";
            using var cmdUsers = new SqliteCommand(qUsers, conn);
            int totalPengguna = Convert.ToInt32(cmdUsers.ExecuteScalar());

            return (totalLab, bookingHariIni, pending, totalPengguna);
        }

        public List<LabUsageStat> GetGrafikPenggunaan()
        {
            var list = new List<LabUsageStat>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string query = @"SELECT NamaLab, COUNT(*) as Total 
                            FROM Booking 
                            GROUP BY NamaLab
                            ORDER BY Total DESC";

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
}
