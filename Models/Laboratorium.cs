namespace LENTERA.WPF.Models
{
    public class Laboratorium
    {
        public int LaboratoriumId { get; set; }
        public string Kode { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Lokasi { get; set; } = string.Empty;
        public int Kapasitas { get; set; }
        public string Fasilitas { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}