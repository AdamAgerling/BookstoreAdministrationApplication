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

        public ObservableCollection<Store> Stores { get; set; }

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
        public ICommand OpenUpdateBookDialogCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public BooksViewModel()
        {
            _dbContext = new BookstoreDbContext();

            Books = new ObservableCollection<Book>(_dbContext.Books.
                Include(b => b.Author).
                Include(b => b.Publisher).
                Include(b => b.Language).
                Include(b => b.Image).ToList());

            OpenAddBookDialogCommand = new AsyncRelayCommand(OpenAddBookDialogAsync);
            OpenUpdateBookDialogCommand = new AsyncRelayCommand(OpenUpdateBookDialogAsync);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);
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
