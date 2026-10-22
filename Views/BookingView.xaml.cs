using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace LENTERA.WPF.Views
{
    public partial class BookingView : UserControl
    {
        public BookingView()
        {
            InitializeComponent();
        }

        /// <summary>Hanya izinkan angka diketik di kolom Jumlah Peserta.</summary>
        private void AngkaSajaInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}
