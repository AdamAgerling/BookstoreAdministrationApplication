using BookstoreAdmin.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace BookstoreAdmin.Model
{
    public class Author : BaseViewModel
    {
        private string _authorName;
        private string _authorLastName;
        private string _authorBirthCountry;
        private DateTime? _authorBirthDate;
        private DateTime? _authorDeathDate;

        [Key]
        public int AuthorId { get; set; }

        public string AuthorName
        {
            get => _authorName;
            set
            {
                _authorName = value;
                OnPropertyChanged(nameof(AuthorName));
            }
        }

        public string AuthorLastName
        {
            get => _authorLastName;
            set
            {
                _authorLastName = value;
                OnPropertyChanged(nameof(AuthorLastName));
            }
        }

        public string AuthorBirthCountry
        {
            get => _authorBirthCountry;
            set
            {
                _authorBirthCountry = value;
                OnPropertyChanged(nameof(AuthorBirthCountry));
            }
        }

        public DateTime? AuthorBirthDate
        {
            get => _authorBirthDate;
            set
            {
                _authorBirthDate = value;
                OnPropertyChanged(nameof(AuthorBirthDate));
            }
        }

        public DateTime? AuthorDeathDate
        {
            get => _authorDeathDate;
            set
            {
                _authorDeathDate = value;
                OnPropertyChanged(nameof(AuthorDeathDate));
            }
        }

        public string FullName => $"{AuthorName} {AuthorLastName}";

        private ObservableCollection<Book> _books = new ObservableCollection<Book>();
        public ObservableCollection<Book> Books
        {
            get => _books;
            set
            {
                _books = value;
                OnPropertyChanged(nameof(Books));
            }
        }
    }
}
