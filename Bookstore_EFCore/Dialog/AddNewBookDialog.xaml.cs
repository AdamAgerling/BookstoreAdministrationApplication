using System.Windows;

namespace BookstoreAdmin.Dialog
{
    public partial class AddNewBookDialog : Window
    {
        public AddNewBookDialog()
        {
            InitializeComponent();
        }

        private void AddNewBookHandler(object sender, RoutedEventArgs e)
        {


            DialogResult = true;
            Close();
        }

        private void CancelCloseHandler(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
