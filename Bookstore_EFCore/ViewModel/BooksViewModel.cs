using BookstoreAdmin.Dialog;
using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BookstoreAdmin.ViewModel
{
    class BooksViewModel : BaseViewModel
    {
        private readonly BookstoreDbContext _dbContext;
        private Book _selectedBook;
        private ObservableCollection<InventoryBalance> _inventoryBalances;

        public ObservableCollection<InventoryBalance> InventoryBalances
        {
            get => _inventoryBalances;
            set
            {
                _inventoryBalances = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Book> Books { get; set; }

        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged(nameof(SelectedBook));
                (DeleteBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(InventoryBalances));
                LoadImageForSelectedBook();
                LoadStoreInventoryForSelectedBook();
            }
        }

        private ImageSource _bookImage;
        public ImageSource BookImage
        {
            get => _bookImage;
            set
            {
                _bookImage = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenAddBookDialogCommand { get; }
        public ICommand AddBookToStockDialogCommand { get; }
        public ICommand RemoveFromBookStockDialogCommand { get; }
        public ICommand OpenUpdateBookDialogCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public BooksViewModel()
        {
            _dbContext = new BookstoreDbContext();

            Books = new ObservableCollection<Book>(_dbContext.Books.
                Include(b => b.Author).
                Include(b => b.Publisher).
                Include(b => b.Language).
                Include(b => b.Image).OrderBy(b => b.BookTitle).ToList());

            OpenAddBookDialogCommand = new AsyncRelayCommand(OpenAddBookDialogAsync);
            OpenUpdateBookDialogCommand = new AsyncRelayCommand(OpenUpdateBookDialogAsync);
            AddBookToStockDialogCommand = new AsyncRelayCommand(() => OpenManageStockDialogAsync(false));
            RemoveFromBookStockDialogCommand = new AsyncRelayCommand(() => OpenManageStockDialogAsync(true));
            DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);
        }

        private async Task OpenManageStockDialogAsync(bool isRemovingStock)
        {
            if (SelectedBook == null)
            {
                MessageBox.Show($"Please select a book to {(isRemovingStock ? "remove from" : "add to")} stock.", "No book selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var stores = new ObservableCollection<Store>(_dbContext.Stores.ToList());
            var tcs = new TaskCompletionSource<InventoryBalance>();
            var dialogViewModel = new ManageStockDialogViewModel(stores, SelectedBook.ISBN13, tcs, isRemovingStock);

            var dialog = new ManageStockDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                var inventoryUpdate = await tcs.Task;

                var inventory = _dbContext.InventoryBalances
                    .FirstOrDefault(i => i.ISBN13 == inventoryUpdate.ISBN13 && i.StoreId == inventoryUpdate.StoreId);

                if (inventory != null)
                {
                    inventory.BookQuantity = Math.Max(0, inventory.BookQuantity + inventoryUpdate.BookQuantity);
                }
                else if (!isRemovingStock)
                {
                    inventoryUpdate.BookQuantity = Math.Max(0, inventoryUpdate.BookQuantity);
                    _dbContext.InventoryBalances.Add(inventoryUpdate);
                }
                else
                {
                    MessageBox.Show("No matching inventory found for this store and book.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    await _dbContext.SaveChangesAsync();
                    LoadStoreInventoryForSelectedBook();
                    MessageBox.Show($"Stock {(isRemovingStock ? "removed" : "added")} successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating stock: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Debug.WriteLine("Stock management dialog was canceled.");
            }
        }

        private void LoadStoreInventoryForSelectedBook()
        {
            if (SelectedBook != null)
            {
                InventoryBalances = new ObservableCollection<InventoryBalance>(
                    _dbContext.InventoryBalances.Include(ib => ib.Store).Where(ib => ib.ISBN13 == SelectedBook.ISBN13).ToList());
            }
            else
            {
                InventoryBalances = new ObservableCollection<InventoryBalance>();
            }
        }

        private void LoadImageForSelectedBook()
        {
            try
            {
                if (_selectedBook?.Image != null
                    && !string.IsNullOrWhiteSpace(_selectedBook.Image.ImageUrl)
                    && !string.IsNullOrWhiteSpace(_selectedBook.Image.ImageId))
                {
                    var fullUrl = $"{_selectedBook.Image.ImageUrl.TrimEnd('/')}/{_selectedBook.Image.ImageId.Trim()}";
                    Debug.WriteLine($"Loading book image from URL: {fullUrl}");

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullUrl, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.EndInit();
                    BookImage = bitmap;
                }
                else
                {
                    Debug.WriteLine($"Missing image data, loading default image.");
                    BookImage = new BitmapImage(new Uri("pack://application:,,,/Assets/no_image.png"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading image: {ex.Message}");
                BookImage = new BitmapImage(new Uri("pack://application:,,,/Assets/no_image.png"));
            }
        }

        public async Task OpenAddBookDialogAsync()
        {
            var authors = new ObservableCollection<Author>(_dbContext.Authors.ToList());
            var publishers = new ObservableCollection<Publisher>(_dbContext.Publishers.ToList());
            var languages = new ObservableCollection<BookLanguage>(_dbContext.BookLanguages.ToList());

            var tcs = new TaskCompletionSource<Book>();
            var dialogViewModel = new AddNewBookDialogViewModel(authors, publishers, languages, tcs);

            dialogViewModel.BookCreated += book =>
            {
                Books.Add(book);
                Debug.WriteLine($"Book added to observable collection: {book.ISBN13}");
                SelectedBook = book;
            };

            var dialog = new AddNewBookDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                Debug.WriteLine("Dialog returned true.");
            }
            else
            {
                Debug.WriteLine("Dialog was canceled.");
            }
        }

        public async Task OpenUpdateBookDialogAsync()
        {
            if (SelectedBook == null)
            {
                MessageBox.Show("Please select a book to update.", "No book selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tcs = new TaskCompletionSource<Book>();
            var dialogViewModel = new UpdateBookDialogViewModel(SelectedBook,
                new ObservableCollection<Author>(_dbContext.Authors.ToList()),
                new ObservableCollection<Publisher>(_dbContext.Publishers.ToList()),
                new ObservableCollection<BookLanguage>(_dbContext.BookLanguages.ToList()),
                tcs);

            var dialog = new UpdateBookDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                var updatedBook = await tcs.Task;
                if (updatedBook != null)
                {
                    try
                    {

                        var bookToUpdate = _dbContext.Books.Include(b => b.Image).FirstOrDefault(b => b.ISBN13 == updatedBook.ISBN13);
                        if (bookToUpdate != null)
                        {
                            bookToUpdate.BookTitle = updatedBook.BookTitle;
                            bookToUpdate.BookPrice = updatedBook.BookPrice;
                            bookToUpdate.BookRelease = updatedBook.BookRelease;
                            bookToUpdate.Author = updatedBook.Author;
                            bookToUpdate.Publisher = updatedBook.Publisher;
                            bookToUpdate.Language = updatedBook.Language;
                            bookToUpdate.Image = bookToUpdate.Image;

                            await _dbContext.SaveChangesAsync();
                            MessageBox.Show($"The book \"{bookToUpdate.BookTitle}\" has been updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            var index = Books.IndexOf(SelectedBook);
                            if (index >= 0)
                            {
                                Books[index] = null;
                                Books[index] = bookToUpdate;
                                SelectedBook = bookToUpdate;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while updating the book: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                Debug.WriteLine("Update dialog was canceled.");
            }
        }

        private void DeleteBook()
        {
            if (SelectedBook == null)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the book '{SelectedBook.BookTitle}'?",
                "Delete Book",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbContext.Books.Remove(SelectedBook);
                    _dbContext.SaveChanges();

                    Books.Remove(SelectedBook);

                    MessageBox.Show("Book deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectedBook = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the book: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private bool CanDeleteBook()
        {
            return SelectedBook != null;
        }
    }
}
