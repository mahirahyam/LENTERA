# LENTERA — Layanan Elektronik Terpadu Reservasi Laboratorium

## Tim Pengembang

| Nama | NIM |
|---|---|
| Denzel Danieks | 5025241228 |
| Mahirah Yasmin Aulia Mawahib | 5025241095 |

**Mata Kuliah:** Pemrograman Berbasis Kerangka Kerja - C  
**Dosen Pengampu:** Fajar Baskoro

LENTERA (*Layanan Elektronik Terpadu Reservasi Laboratorium*) adalah aplikasi desktop berbasis Windows yang dirancang untuk mempermudah proses pemesanan dan pengelolaan reservasi laboratorium di lingkungan institusi pendidikan.

Aplikasi ini mengintegrasikan proses pengajuan booking, pemeriksaan ketersediaan laboratorium, persetujuan reservasi, pengelolaan data laboratorium, serta pemantauan penggunaan laboratorium dalam satu sistem.

## Latar Belakang

Proses pemesanan laboratorium secara manual melalui chat, formulir, atau komunikasi lisan berisiko menimbulkan bentrok jadwal dan menyulitkan pengelolaan data reservasi.

LENTERA dikembangkan sebagai solusi untuk membantu pengguna memeriksa ketersediaan laboratorium, mengajukan reservasi, dan memantau status booking. Di sisi pengelola, aplikasi menyediakan fitur untuk memproses permohonan dan memantau penggunaan laboratorium secara lebih terstruktur.

## Tujuan Pengembangan

- Mempermudah proses reservasi laboratorium.
- Mencegah bentrok jadwal pemakaian laboratorium.
- Mempercepat proses persetujuan booking.
- Menyediakan informasi penggunaan laboratorium secara terpusat.
- Menerapkan pengembangan aplikasi desktop yang terstruktur dan mudah dipelihara.

## Fitur Utama

### 1. Dashboard
- Menampilkan ringkasan statistik penggunaan laboratorium.
- Menampilkan jumlah laboratorium aktif, booking hari ini, booking yang menunggu persetujuan, dan total pengguna.
- Menyajikan grafik penggunaan laboratorium.
- Menampilkan aktivitas terbaru yang relevan dengan proses reservasi.

### 2. Booking dan Reservasi
- Membuat pengajuan reservasi laboratorium.
- Melihat detail booking dan status pengajuan.
- Mengubah booking milik pengguna selama masih berstatus Menunggu.
- Membatalkan booking dengan konfirmasi.
- Mencari booking berdasarkan kode booking, nama peminjam, atau nama laboratorium.
- Memfilter data berdasarkan status, laboratorium, dan tanggal.

### 3. Persetujuan Booking
- Memproses permohonan reservasi oleh laboran atau administrator.
- Menyetujui atau menolak pengajuan booking.
- Mewajibkan alasan ketika pengajuan ditolak.
- Memperbarui status booking setelah keputusan diberikan.

### 4. Jadwal Laboratorium
- Melihat ketersediaan laboratorium berdasarkan tanggal.
- Memeriksa slot waktu yang telah terisi maupun yang masih tersedia.
- Membantu pengguna memilih waktu reservasi.
- Mengurangi risiko pemesanan pada jadwal yang bertabrakan.

### 5. Pengelolaan Laboratorium
- Melihat daftar laboratorium.
- Mengelola nama, lokasi, kapasitas, fasilitas, dan status aktif laboratorium.
- Menambah dan mengubah data laboratorium.
- Menonaktifkan laboratorium yang tidak digunakan.

### 6. Manajemen Pengguna
- Mengelola akun pengguna oleh administrator.
- Menambah dan mengubah data akun.
- Menetapkan peran pengguna.
- Menonaktifkan akun yang tidak lagi digunakan.
- Membatasi akses fitur berdasarkan peran pengguna.

### 7. Laporan
- Menampilkan rekap penggunaan laboratorium.
- Menyaring laporan berdasarkan periode dan laboratorium.
- Menyediakan ringkasan data untuk membantu evaluasi penggunaan laboratorium.

### 8. Pengaturan dan Antarmuka
- Menyediakan navigasi melalui sidebar.
- Mendukung pilihan tema gelap (*dark mode*) dan terang (*light mode*).
- Menyediakan validasi formulir dan notifikasi hasil operasi.
- Menampilkan pesan kesalahan yang mudah dipahami.

## Alur Reservasi

1. Pengguna masuk ke aplikasi menggunakan akun yang valid.
2. Pengguna melihat dashboard dan memeriksa jadwal laboratorium.
3. Pengguna memilih laboratorium, tanggal, jam mulai, jam selesai, dan mengisi keperluan reservasi.
4. Sistem memvalidasi data dan memeriksa kemungkinan bentrok jadwal.
5. Pengajuan disimpan dengan status **Menunggu**.
6. Laboran atau administrator memeriksa pengajuan dan menyetujui atau menolaknya.
7. Jika ditolak, alasan penolakan dicatat.
8. Status booking diperbarui agar dapat dipantau oleh pengguna.

## Status Booking

| Status | Keterangan |
|---|---|
| Menunggu | Pengajuan menunggu keputusan. |
| Disetujui | Reservasi dikonfirmasi dan slot waktu dikunci. |
| Ditolak | Pengajuan tidak disetujui dan alasan penolakan dicatat. |
| Dibatalkan | Reservasi dibatalkan oleh peminjam atau administrator. |
| Selesai | Waktu penggunaan laboratorium telah berlalu. |

## Peran Pengguna

| Peran | Tanggung Jawab |
|---|---|
| Administrator | Mengelola akun, data laboratorium, dan konfigurasi aplikasi. |
| Laboran | Memverifikasi booking, mengelola jadwal, dan melihat laporan. |
| Dosen | Mengajukan dan membatalkan booking miliknya serta melihat jadwal laboratorium. |
| Mahasiswa | Mengajukan dan membatalkan booking miliknya serta melihat jadwal laboratorium. |

Hak akses setiap peran mengikuti aturan yang ditetapkan dalam implementasi aplikasi.

## Teknologi

- **C#** — bahasa pemrograman utama.
- **WPF (Windows Presentation Foundation)** — pengembangan antarmuka desktop.
- **XAML** — pendefinisian tampilan dan komponen UI.
- **.NET** — platform pengembangan aplikasi.
- **ADO.NET** — akses dan pengelolaan data melalui koneksi database.
- **SQLite** — opsi penyimpanan database lokal pada pengembangan.
- **Git & GitHub** — version control dan kolaborasi pengembangan.

## Data dan Validasi

Sistem dirancang untuk menggunakan database sebagai sumber data utama. Data booking harus divalidasi sebelum disimpan, termasuk kelengkapan formulir, urutan jam, tanggal reservasi, kapasitas laboratorium, jam operasional, dan kemungkinan bentrok jadwal.

Sistem juga dirancang untuk menggunakan query berparameter serta penanganan kesalahan database agar data lebih aman dan aplikasi lebih andal.

## 📄 Dokumentasi

---

*LENTERA — Layanan Elektronik Terpadu Reservasi Laboratorium*