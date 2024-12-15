using System.Windows;

namespace BookstoreAdmin.Dialog.Store
{
    public partial class ManageExistingBookInStoreDialog : Window
    {
        public ManageExistingBookInStoreDialog()
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
