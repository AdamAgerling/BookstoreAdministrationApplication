using BookstoreAdmin.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace BookstoreAdmin.Model
{
    public class Publisher : BaseViewModel
    {
        private string _publisherName;
        private string _publisherCountry;
        private int? _publisherFoundedYear;

        [Key]
        public int PublisherId { get; set; }
        public string PublisherName
        {
            get => _publisherName;
            set
            {
                _publisherName = value;
                OnPropertyChanged(nameof(PublisherName));
            }
        }

        public string PublisherCountry
        {
            get => _publisherCountry;
            set
            {
                _publisherCountry = value;
                OnPropertyChanged(nameof(PublisherCountry));
            }
        }

        public int? PublisherFoundedYear
        {
            get => _publisherFoundedYear;
            set
            {
                _publisherFoundedYear = value;
                OnPropertyChanged(nameof(PublisherFoundedYear));
            }
        }

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