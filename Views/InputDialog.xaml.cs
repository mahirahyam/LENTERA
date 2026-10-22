using System.Windows;

namespace LENTERA.WPF.Views
{
    public partial class InputDialog : Window
    {
        public string InputText => InputBox.Text;

        public InputDialog(string title, string message, string placeholder = "")
        {
            InitializeComponent();
            TitleText.Text = title;
            MessageText.Text = message;
            InputBox.Text = placeholder;
            Loaded += (_, _) => InputBox.Focus();
        }

        private void Simpan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputBox.Text))
            {
                MessageBox.Show("Alasan tidak boleh kosong.", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }

        private void Batal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
