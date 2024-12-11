using BookstoreAdmin.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace BookstoreAdmin.ViewModel
{
    internal class AuthorsViewModel : BaseViewModel
    {
        private readonly BookstoreDbContext _dbContext;
        private Author _selectedAuthor;
        public ObservableCollection<Author> Authors { get; set; }

        public Author SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged(nameof(SelectedAuthor));
            }
        }
        public AuthorsViewModel()
        {
            _dbContext = new BookstoreDbContext();

            Authors = new ObservableCollection<Author>(_dbContext.Authors.
                Include(a => a.Books).OrderBy(a => a.AuthorName).ToList());
        }
    }
}
