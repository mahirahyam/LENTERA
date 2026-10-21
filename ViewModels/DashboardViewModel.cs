using System.Collections.ObjectModel;

namespace LENTERA.WPF.Models
{
    public class LabUsageStat
    {
        public string NamaLab { get; set; } = string.Empty;
        public int TotalBooking { get; set; }
        public double BarHeight => TotalBooking * 15;
    }
}

namespace LENTERA.WPF.ViewModels
{
    using LENTERA.WPF.Models;

    public class DashboardViewModel : BaseViewModel
    {
        public int TotalLabAktif { get; set; } = 8;
        public int BookingHariIni { get; set; } = 5;
        public int MenungguPersetujuan { get; set; } = 3;
        public int TotalPengguna { get; set; } = 142;

        public ObservableCollection<LabUsageStat> GrafikPenggunaan { get; set; }

        public DashboardViewModel()
        {
            GrafikPenggunaan = new ObservableCollection<LabUsageStat>
            {
                new LabUsageStat { NamaLab = "Lab Pemrograman", TotalBooking = 12 },
                new LabUsageStat { NamaLab = "Lab Jaringan", TotalBooking = 8 },
                new LabUsageStat { NamaLab = "Lab Multimedia", TotalBooking = 10 },
                new LabUsageStat { NamaLab = "Lab Sistem Informasi", TotalBooking = 5 }
            };
        }
    }
}