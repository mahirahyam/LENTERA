using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace LENTERA.WPF.Views
{
    public partial class ReservationView : UserControl
    {
        public ReservationView() { InitializeComponent(); }
        private void AngkaSajaInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}
