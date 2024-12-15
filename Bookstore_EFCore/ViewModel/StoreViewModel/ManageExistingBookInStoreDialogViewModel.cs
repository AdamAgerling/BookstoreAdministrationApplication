using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel.StoreViewModel
{
    class ManageExistingBookInStoreDialogViewModel : BaseViewModel
    {
        private readonly TaskCompletionSource<(Book, int, bool)> _tcs;
        private Book _selectedBook;
        private int _quantity;
        private bool _removeEntirely;
        private readonly DialogMode _mode;
        public string DialogTitle => _mode == DialogMode.Add ? "Add Book to Inventory" : "Remove Book from Inventory";
        public string ConfirmButtonText => _mode == DialogMode.Add ? "Add" : "Remove";
        public enum DialogMode
        {
            Add,
            Remove
        }

        public ObservableCollection<Book> AvailableBooks { get; set; }
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (_selectedBook != value)
                {
                    _selectedBook = value;
                    OnPropertyChanged(nameof(SelectedBook));
                    (RemoveOrAddBookCommand as RelayCommand<Window>)?.NotifyCanExecuteChanged();
                }
                else
                {
                    OnPropertyChanged(nameof(SelectedBook));
                    (RemoveOrAddBookCommand as RelayCommand<Window>)?.NotifyCanExecuteChanged();
                }
            }
        }
        public bool RemoveEntirely
        {
            get => _removeEntirely;
            set
            {
                _removeEntirely = value;
                OnPropertyChanged(nameof(RemoveEntirely));
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public ICommand RemoveOrAddBookCommand { get; }

        public ManageExistingBookInStoreDialogViewModel(ObservableCollection<Book> availableBooks, TaskCompletionSource<(Book, int, bool)> tcs, DialogMode mode)
        {
            _tcs = tcs;
            AvailableBooks = availableBooks;
            _mode = mode;
            RemoveOrAddBookCommand = new RelayCommand<Window>(RemoveOrAddBookAction, CanRemoveOrAddBookAction);
        }

        private void RemoveOrAddBookAction(Window window)
        {
            _tcs.SetResult((SelectedBook, Quantity, RemoveEntirely));
            window.DialogResult = true;
        }
        private bool CanRemoveOrAddBookAction(Window window)
        {
            if (!int.TryParse(Quantity.ToString(), out int parsedQuantity) || parsedQuantity < 0)
            {
                return false;
            }

            if (_mode == DialogMode.Add)
            {
                RemoveEntirely = false;
                return SelectedBook != null && parsedQuantity >= 0;
            }
            else if (_mode == DialogMode.Remove)
            {
                return SelectedBook != null && (parsedQuantity >= 0 || RemoveEntirely);
            }
            return false;
        }
        public bool IsRemoveMode => _mode == DialogMode.Remove;
    }
}
