namespace LENTERA.WPF.Models
{
    /// <summary>Statistik jumlah booking per laboratorium, dipakai oleh grafik di Dashboard.</summary>
    public class LabUsageStat
    {
        public string NamaLab { get; set; } = string.Empty;
        public int TotalBooking { get; set; }

        /// <summary>Tinggi batang grafik (px). Minimal 6px agar batang dengan nilai 0 tetap terlihat.</summary>
        public double BarHeight => TotalBooking > 0 ? TotalBooking * 15 : 6;
    }
}
