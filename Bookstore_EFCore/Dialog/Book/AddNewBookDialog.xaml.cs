using System.Windows;

namespace BookstoreAdmin.Dialog.Book
{
    public partial class AddNewBookDialog : Window
    {
        public AddNewBookDialog()
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
