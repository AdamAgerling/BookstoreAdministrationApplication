using System.Windows;

namespace BookstoreAdmin.Dialog.Book
{
    public partial class ManageStockDialog : Window
    {
        public ManageStockDialog()
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
