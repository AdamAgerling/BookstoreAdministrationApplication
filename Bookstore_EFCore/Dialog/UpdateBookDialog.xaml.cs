using System.Windows;

namespace BookstoreAdmin.Dialog
{
    /// <summary>
    /// Interaction logic for UpdateBookDialog.xaml
    /// </summary>
    public partial class UpdateBookDialog : Window
    {
        public UpdateBookDialog()
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
