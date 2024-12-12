using System.Windows;

namespace BookstoreAdmin.Dialog.Author
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
