using BookstoreAdmin.Dialog;
using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BookstoreAdmin.ViewModel
{
    class BooksViewModel : BaseViewModel
    {
        private readonly BookstoreDbContext _dbContext;
        private Book _selectedBook;
        public ObservableCollection<Book> Books { get; set; }
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged(nameof(SelectedBook));
                LoadImageForSelectedBook();
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
        public ICommand UpdateBookCommand { get; }
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
            UpdateBookCommand = new RelayCommand(UpdateBook);
            DeleteBookCommand = new RelayCommand(DeleteBook);


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

        public void UpdateBook()
        {
            if (SelectedBook != null)
            {
                _dbContext.SaveChanges();
            }
        }

        public void DeleteBook()
        {
            if (SelectedBook != null)
            {
                _dbContext.Books.Remove(SelectedBook);
                _dbContext.SaveChanges();
                Books.Remove(SelectedBook);
            }
        }
        private bool CanUpdateOrDelete()
        {
            return SelectedBook != null;
        }
    }
}
