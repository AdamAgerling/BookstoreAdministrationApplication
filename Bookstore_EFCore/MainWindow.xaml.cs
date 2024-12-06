using BookstoreAdmin.ViewModel;
using System.Windows;

namespace BookstoreAdmin
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new BooksViewModel();
        }
    }
}
