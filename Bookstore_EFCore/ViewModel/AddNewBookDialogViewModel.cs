using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel
{
    internal class AddNewBookDialogViewModel : BaseViewModel
    {
        private string _newBookISBN;
        private string _newBookTitle;
        private decimal _newBookPrice;
        private DateTime _newBookRelease;
        private Author _selectedAuthor;
        private Publisher _selectedPublisher;
        private BookLanguage _selectBookLanguage;
        private ObservableCollection<Author> _authors;
        private ObservableCollection<Publisher> _publishers;
        private ObservableCollection<BookLanguage> _languages;
        private readonly TaskCompletionSource<Book> _tcs;
        public event Action<Book> BookCreated;

        public string NewBookISBN
        {
            get => _newBookISBN;
            set
            {
                _newBookISBN = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public string NewBookTitle
        {
            get => _newBookTitle;
            set
            {
                _newBookTitle = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public decimal NewBookPrice
        {
            get => _newBookPrice;
            set
            {
                _newBookPrice = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public DateTime NewBookRelease
        {
            get => _newBookRelease;
            set
            {
                _newBookRelease = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
        public Author SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<Author> Authors
        {
            get => _authors;
            set
            {
                _authors = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public Publisher SelectedPublisher
        {
            get => _selectedPublisher;
            set
            {
                _selectedPublisher = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
        public ObservableCollection<Publisher> Publishers
        {
            get => _publishers;
            set
            {
                _publishers = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public BookLanguage SelectedBookLanguage
        {
            get => _selectBookLanguage;
            set
            {
                _selectBookLanguage = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<BookLanguage> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                OnPropertyChanged();
                (CreateBookCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public ICommand CreateBookCommand { get; }
        public ICommand CancelCommand { get; }

        public AddNewBookDialogViewModel(
            ObservableCollection<Author> authors,
            ObservableCollection<Publisher> publishers,
            ObservableCollection<BookLanguage> langugages,
            TaskCompletionSource<Book> tcs)
        {
            Authors = authors ?? new ObservableCollection<Author>();
            Publishers = publishers ?? new ObservableCollection<Publisher>();
            Languages = langugages ?? new ObservableCollection<BookLanguage>();
            _tcs = tcs;

            NewBookISBN = string.Empty;
            NewBookTitle = string.Empty;
            NewBookPrice = 0;
            NewBookRelease = DateTime.Now;
            SelectedAuthor = Authors.FirstOrDefault();
            SelectedPublisher = Publishers.FirstOrDefault();
            SelectedBookLanguage = Languages.FirstOrDefault();


            CreateBookCommand = new RelayCommand(CreateBook, CanCreateBook);
            CancelCommand = new RelayCommand(Cancel);
        }

        private async void CreateBook()
        {
            try
            {
                if (!string.IsNullOrEmpty(NewBookISBN) &&
                    !string.IsNullOrEmpty(NewBookTitle) &&
                    SelectedAuthor != null &&
                    SelectedPublisher != null &&
                    SelectedBookLanguage != null)
                {
                    var scraper = new BokusBookImageScraper(NewBookISBN);
                    await scraper.SaveImageToDataBaseAsync();

                    Debug.WriteLine("Image saving process completed. Creating new book...");

                    using (var db = new BookstoreDbContext())
                    {
                        var image = await db.Images.FindAsync(NewBookISBN);
                        if (image == null)
                        {
                            Debug.WriteLine("Image not found in database. Cannot create book.");
                            return;
                        }

                        var existingBook = db.Books.Include(b => b.Image).FirstOrDefault(b => b.ISBN13 == NewBookISBN);
                        if (existingBook != null)
                        {
                            Debug.WriteLine("A book with the same ISBN already exists. Aborting creation.");
                            return;
                        }

                        var author = db.Authors.FirstOrDefault(a => a.AuthorId == SelectedAuthor.AuthorId) ?? SelectedAuthor;

                        var publisher = db.Publishers.FirstOrDefault(p => p.PublisherId == SelectedPublisher.PublisherId) ?? SelectedPublisher;

                        var language = db.BookLanguages.FirstOrDefault(l => l.LanguageId == SelectedBookLanguage.LanguageId) ?? SelectedBookLanguage;

                        var newBook = new Book
                        {
                            ISBN13 = NewBookISBN,
                            BookTitle = NewBookTitle,
                            BookPrice = NewBookPrice,
                            BookRelease = NewBookRelease,
                            Author = author,
                            Publisher = publisher,
                            Language = language,
                            Image = image
                        };

                        db.Books.Add(newBook);
                        await db.SaveChangesAsync();

                        Debug.WriteLine($"New book added successfully: {newBook.ISBN13} - {newBook.BookTitle}");

                        BookCreated?.Invoke(newBook);

                        _tcs.SetResult(newBook);
                        Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                    }
                }
                else
                {
                    Debug.WriteLine("Validation failed. Inputs are not valid.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateBook: {ex.Message}");
            }
        }

        public void Cancel()
        {
            Debug.WriteLine("Cancel command executed.");

            if (!_tcs.Task.IsCompleted)
            {
                Debug.WriteLine("Setting null result for TaskCompletionSource.");
                _tcs.SetResult(null);
            }
            else
            {
                Debug.WriteLine("TaskCompletionSource is already completed.");
            }
        }

        private bool CanCreateBook()
        {
            return !string.IsNullOrWhiteSpace(NewBookISBN) &&
                   !string.IsNullOrWhiteSpace(NewBookTitle) &&
                   NewBookPrice > 0 &&
                   SelectedAuthor != null &&
                   SelectedPublisher != null &&
                   SelectedBookLanguage != null &&
                   NewBookRelease != default;
        }
    }
}