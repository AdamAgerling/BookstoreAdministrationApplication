using System.Windows;

namespace BookstoreAdmin.Dialog.Publisher
{
    public partial class AddNewPublisherDialog : Window
    {
        public AddNewPublisherDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void YearTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
