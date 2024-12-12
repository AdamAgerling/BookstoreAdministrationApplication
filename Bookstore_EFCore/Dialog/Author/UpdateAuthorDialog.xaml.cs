using System.Windows;

namespace BookstoreAdmin.Dialog.Author
{
    public partial class UpdateAuthorDialog : Window
    {
        public UpdateAuthorDialog()
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
