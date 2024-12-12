using System.Windows;

namespace BookstoreAdmin.Dialog
{
    public partial class AddNewAuthorDialog : Window
    {
        public AddNewAuthorDialog()
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
