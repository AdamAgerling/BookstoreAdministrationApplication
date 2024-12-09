using BookstoreAdmin.ViewModel;
using System.Windows;

namespace BookstoreAdmin.Dialog
{
    public partial class AddNewBookDialog : Window
    {
        public AddNewBookDialog()
        {
            InitializeComponent();
            Closing += AddNewBookDialog_Closing;
        }

        private void AddNewBookDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is AddNewBookDialogViewModel dialog)
            {
                dialog.Cancel();
            }
        }

    }
}
