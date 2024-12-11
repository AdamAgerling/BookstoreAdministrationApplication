using System.Windows;

namespace BookstoreAdmin.Dialog
{
    public partial class UpdateBookDialog : Window
    {
        public UpdateBookDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
