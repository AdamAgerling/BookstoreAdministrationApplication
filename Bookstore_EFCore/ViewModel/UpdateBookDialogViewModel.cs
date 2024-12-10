using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel
{
    public class UpdateBookDialogViewModel : BaseViewModel
    {
        private readonly TaskCompletionSource<Book> _tcs;

        public string NewBookISBN { get; }
        public string NewBookTitle { get; set; }
        public decimal NewBookPrice { get; set; }
        public Image SelectedImage { get; set; }
        public DateTime NewBookRelease { get; set; }
        public Author SelectedAuthor { get; set; }
        public Publisher SelectedPublisher { get; set; }
        public BookLanguage SelectedBookLanguage { get; set; }
        public ObservableCollection<Author> Authors { get; }
        public ObservableCollection<Publisher> Publishers { get; }
        public ObservableCollection<BookLanguage> Languages { get; }

        public ICommand UpdateBookCommand { get; }

        public UpdateBookDialogViewModel(Book selectedBook,
            ObservableCollection<Author> authors,
            ObservableCollection<Publisher> publishers,
            ObservableCollection<BookLanguage> languages,
            TaskCompletionSource<Book> tcs)
        {
            _tcs = tcs;

            NewBookISBN = selectedBook.ISBN13;
            NewBookTitle = selectedBook.BookTitle;
            NewBookPrice = selectedBook.BookPrice;
            NewBookRelease = selectedBook.BookRelease;
            SelectedAuthor = selectedBook.Author;
            SelectedPublisher = selectedBook.Publisher;
            SelectedBookLanguage = selectedBook.Language;

            Authors = authors;
            Publishers = publishers;
            Languages = languages;

            UpdateBookCommand = new RelayCommand<Window>(UpdateBook);
        }

        private void UpdateBook(Window window)
        {

            if (string.IsNullOrWhiteSpace(NewBookTitle) || SelectedAuthor == null || SelectedPublisher == null || SelectedBookLanguage == null)
            {
                MessageBox.Show("All fields must be filled to update the book.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updatedBook = new Book
            {
                ISBN13 = NewBookISBN,
                BookTitle = NewBookTitle,
                BookPrice = NewBookPrice,
                BookRelease = NewBookRelease,
                Author = SelectedAuthor,
                Publisher = SelectedPublisher,
                Language = SelectedBookLanguage,
                Image = SelectedImage
            };

            if (window != null)
            {
                _tcs.SetResult(updatedBook);
                window.DialogResult = true;
            }
            else
            {
                MessageBox.Show("An unexpected error occurred: Unable to close the dialog.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
