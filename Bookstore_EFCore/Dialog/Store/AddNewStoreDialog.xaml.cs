using System.Windows;

namespace BookstoreAdmin.Dialog.Store
{
    public partial class AddNewStoreDialog : Window
    {
        public AddNewStoreDialog()
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
