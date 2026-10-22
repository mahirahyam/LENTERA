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
            using var transaction = conn.BeginTransaction();
            string seedQuery = @"
                INSERT INTO Laboratorium (Kode, Nama, Lokasi, Kapasitas, Fasilitas, IsActive) VALUES
                ('LAB-RPL', 'Lab. Rekayasa Perangkat Lunak', 'Gedung A Lt. 2', 35, '35 PC, AC, Proyektor', 1),
                ('LAB-KBJ', 'Lab. Komputasi Berbasis Jaringan', 'Gedung A Lt. 2', 30, '30 PC, Switch, Router Cisco', 1),
                ('LAB-KCV', 'Lab. Komputasi Cerdas dan Visi', 'Gedung A Lt. 3', 25, '25 PC Workstation GPU', 1),
                ('LAB-TJKS', 'Lab. Teknologi Jaringan dan Keamanan Siber Cerdas', 'Gedung A Lt. 3', 25, '25 PC, Firewall Lab Kit', 1),
                ('LAB-GIGA', 'Lab. Grafika, Interaksi, Gim dan Analitik', 'Gedung B Lt. 1', 30, '30 PC Graphic Workstation, VR Set', 1),
                ('LAB-AP', 'Lab. Algoritma dan Pemrograman', 'Gedung B Lt. 1', 40, '40 PC, Proyektor, Whiteboard', 1),
                ('LAB-MCI', 'Lab. Manajemen Cerdas Informasi', 'Gedung B Lt. 2', 30, '30 PC, Server Database', 1),
                ('LAB-PKT', 'Lab. Pemodelan dan Komputasi Terapan', 'Gedung B Lt. 2', 25, '25 PC, Software Simulasi', 1),
                ('LAB-09', 'Lab. Komputasi dan Data', 'Gedung C Lt. 1', 25, '25 PC, Server Database', 1),
                ('LAB-10', 'Lab. Sistem dan Teknologi Informasi', 'Gedung C Lt. 2', 20, '20 PC, Proyektor', 1)
                ON CONFLICT(Kode) DO UPDATE SET Nama=excluded.Nama, Lokasi=excluded.Lokasi,
                    Kapasitas=excluded.Kapasitas, Fasilitas=excluded.Fasilitas, IsActive=1;

                DELETE FROM Laboratorium WHERE Kode IN ('WS-01','WS-02','LAB-S2','LAB-S3');
                UPDATE Booking SET NamaLab='Lab. Komputasi dan Data' WHERE NamaLab IN ('Workshop Pemrograman 1','Workshop Pemrograman 2','Lab. Pascasarjana S3');
                UPDATE Booking SET NamaLab='Lab. Sistem dan Teknologi Informasi' WHERE NamaLab='Lab. Pascasarjana S2';
            ";
            using var cmd = new SqliteCommand(seedQuery, conn, transaction);
            cmd.ExecuteNonQuery();

            // Seed setiap booking contoh secara idempoten. Jika database sudah memiliki
            // sebagian data (misalnya hanya BK-001 dan BK-002), data yang belum ada
            // tetap ditambahkan tanpa menduplikasi booking yang sudah tersimpan.
            string bookingSeed = @"
                    INSERT OR IGNORE INTO Booking (KodeBooking,NamaPeminjam,NamaLab,Tanggal,JamMulai,JamSelesai,Keperluan,JumlahPeserta,Status,AlasanPenolakan) VALUES
                    ('BK-001','Mahirah Yasmin','Lab. Algoritma dan Pemrograman',date('now'),'08:00','10:00','Praktikum Algoritma',35,'Disetujui',''),
                    ('BK-002','Budi Santoso','Lab. Komputasi Berbasis Jaringan',date('now'),'10:00','12:00','Ujian Sertifikasi Jaringan',25,'Menunggu',''),
                    ('BK-003','Citra Dewi','Lab. Rekayasa Perangkat Lunak',date('now','+1 day'),'13:00','15:00','Demo Proyek Akhir',20,'Disetujui',''),
                    ('BK-004','Dimas Prakoso','Lab. Komputasi Cerdas dan Visi',date('now','+1 day'),'08:00','10:00','Riset Computer Vision',10,'Menunggu',''),
                    ('BK-005','Erlangga Putra','Lab. Teknologi Jaringan dan Keamanan Siber Cerdas',date('now','+2 day'),'09:00','11:00','Pelatihan Keamanan Siber',18,'Ditolak','Jadwal bentrok dengan kegiatan lab rutin'),
                    ('BK-006','Fitriani Rahma','Lab. Grafika, Interaksi, Gim dan Analitik',date('now','+2 day'),'13:00','16:00','Pengembangan Game Edukasi',22,'Disetujui',''),
                    ('BK-007','Gilang Ramadhan','Lab. Manajemen Cerdas Informasi',date('now','-1 day'),'08:00','10:00','Workshop Basis Data',28,'Disetujui',''),
                    ('BK-008','Hana Permata','Lab. Pemodelan dan Komputasi Terapan',date('now','+3 day'),'10:00','12:00','Simulasi Numerik',15,'Menunggu',''),
                    ('BK-009','Irfan Maulana','Lab. Komputasi dan Data',date('now','+3 day'),'13:00','15:00','Pelatihan Python Dasar',20,'Menunggu',''),
                    ('BK-010','Jihan Salsabila','Lab. Sistem dan Teknologi Informasi',date('now','+4 day'),'09:00','11:00','Seminar Proposal Tesis',12,'Disetujui','');";
            using var bookingCmd = new SqliteCommand(bookingSeed, conn, transaction);
            bookingCmd.ExecuteNonQuery();

            transaction.Commit();
        }

    }
}