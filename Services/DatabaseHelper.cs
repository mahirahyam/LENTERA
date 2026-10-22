using Microsoft.Data.Sqlite;
using System.IO;

namespace LENTERA.WPF.Services
{
    public static class DatabaseHelper
    {
        private const string DbFileName = "lentera.db";
        private static readonly string ConnectionString = $"Data Source={DbFileName}";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }

        public static void InitializeDatabase()
        {
            using var conn = GetConnection();
            conn.Open();

            string createTablesQuery = @"
                CREATE TABLE IF NOT EXISTS Laboratorium (
                    LaboratoriumId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Kode TEXT NOT NULL UNIQUE,
                    Nama TEXT NOT NULL,
                    Lokasi TEXT NOT NULL,
                    Kapasitas INTEGER NOT NULL,
                    Fasilitas TEXT,
                    IsActive INTEGER DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS Booking (
                    BookingId INTEGER PRIMARY KEY AUTOINCREMENT,
                    KodeBooking TEXT NOT NULL UNIQUE,
                    NamaPeminjam TEXT NOT NULL,
                    NamaLab TEXT NOT NULL,
                    Tanggal TEXT NOT NULL,
                    JamMulai TEXT NOT NULL,
                    JamSelesai TEXT NOT NULL,
                    Keperluan TEXT NOT NULL,
                    JumlahPeserta INTEGER NOT NULL,
                    Status TEXT NOT NULL DEFAULT 'Menunggu',
                    AlasanPenolakan TEXT
                );
            ";

            using var cmd = new SqliteCommand(createTablesQuery, conn);
            cmd.ExecuteNonQuery();

            SeedDefaultData(conn);
        }

        private static void SeedDefaultData(SqliteConnection conn)
        {
            string checkQuery = "SELECT COUNT(*) FROM Laboratorium";
            using var checkCmd = new SqliteCommand(checkQuery, conn);
            long count = (long)(checkCmd.ExecuteScalar() ?? 0);

            if (count == 0)
            {
                string seedQuery = @"
                    INSERT INTO Laboratorium (Kode, Nama, Lokasi, Kapasitas, Fasilitas, IsActive) VALUES
                    ('LAB-01', 'Lab Pemrograman', 'Gedung A Lt. 2', 40, '40 PC, AC, Proyektor', 1),
                    ('LAB-02', 'Lab Jaringan Komputer', 'Gedung A Lt. 3', 30, '30 PC, Router Cisco', 1),
                    ('LAB-03', 'Lab Multimedia', 'Gedung B Lt. 1', 35, '35 PC Graphic Workstation', 1),
                    ('LAB-04', 'Lab Sistem Informasi', 'Gedung B Lt. 2', 40, '40 PC, Whiteboard', 1);

                    INSERT INTO Booking (KodeBooking, NamaPeminjam, NamaLab, Tanggal, JamMulai, JamSelesai, Keperluan, JumlahPeserta, Status) VALUES
                    ('BK-001', 'Mahirah Yasmin', 'Lab Pemrograman', date('now'), '08:00:00', '10:00:00', 'Praktikum Pemrograman', 35, 'Disetujui'),
                    ('BK-002', 'Budi Santoso', 'Lab Jaringan Komputer', date('now'), '10:00:00', '12:00:00', 'Ujian Sertifikasi', 25, 'Menunggu');
                ";
                using var seedCmd = new SqliteCommand(seedQuery, conn);
                seedCmd.ExecuteNonQuery();
            }
        }
    }
}