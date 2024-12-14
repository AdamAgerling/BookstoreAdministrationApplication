using System.Windows;

namespace BookstoreAdmin.Dialog.Store
{
    public partial class AddExistingBookToStoreDialog : Window
    {
        public AddExistingBookToStoreDialog()
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
