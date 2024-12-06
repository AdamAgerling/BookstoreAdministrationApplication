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

        public ICommand AddBookCommand { get; }
        public ICommand UpdateBookCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public BooksViewModel()
        {
            _dbContext = new BookstoreDbContext();

            Books = new ObservableCollection<Book>(_dbContext.Books.
                Include(b => b.Author).
                Include(b => b.Publisher).
                Include(b => b.Language).ToList());

            AddBookCommand = new RelayCommand(AddBook);
            UpdateBookCommand = new RelayCommand(UpdateBook);
            DeleteBookCommand = new RelayCommand(DeleteBook);


        }

        private readonly string DefaultImagePath = "pack://application:,,,/Assets/no_image.png";

        private void LoadImageForSelectedBook()
        {
            try
            {
                if (_selectedBook?.Image?.ImageUrl is string baseUrl
                    && _selectedBook?.Image?.Id is string imageId
                    && !string.IsNullOrWhiteSpace(baseUrl)
                    && !string.IsNullOrWhiteSpace(imageId))
                {
                    // FUNGERAR INTE ÄN HMM 
                    var fullUrl = $"{baseUrl}{imageId}";
                    Debug.WriteLine($"Loading book image from URL: {fullUrl}");
                    BookImage = new BitmapImage(new Uri(fullUrl, UriKind.Absolute));
                }
                else
                {
                    Debug.WriteLine("Loading default image.");
                    BookImage = new BitmapImage(new Uri("pack://application:,,,/Assets/no_image.png"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading image: {ex.Message}");
                BookImage = new BitmapImage(new Uri("pack://application:,,,/Assets/no_image.png"));
            }
        }

        public void AddBook()
        {
            if (!_dbContext.Authors.Any() || !_dbContext.BookLanguages.Any() || !_dbContext.Publishers.Any())
            {
                MessageBox.Show("NOT ENOUGH INFORMATION GIVEN");
                return;
            }

            var newBook = new Book
            {
                ISBN13 = "978-0-1234-5678-9",
                BookTitle = "New Book Title",
                BookPrice = 9.99M,
                BookRelease = DateTime.Now,
                Language = _dbContext.BookLanguages.First(),
                Author = _dbContext.Authors.First(),
                Publisher = _dbContext.Publishers.First()
            };
            _dbContext.Books.Add(newBook);
            _dbContext.SaveChanges();
            Books.Add(newBook);
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
