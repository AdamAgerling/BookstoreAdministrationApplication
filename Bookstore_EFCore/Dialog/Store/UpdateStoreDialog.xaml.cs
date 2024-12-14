using System.Windows;

namespace BookstoreAdmin.Dialog.Store
{
    public partial class UpdateStoreDialog : Window
    {
        public UpdateStoreDialog()
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
