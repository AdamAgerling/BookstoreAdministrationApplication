using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel
{
    class BookstoreViewModel : BaseViewModel
    {
        private readonly BookstoreDbContext _dbContext;

        public ObservableCollection<Book> Books { get; set; }
        public Book SelectedBook { get; set; }
        public ObservableCollection<Store> Stores { get; set; }


        public ICommand AddBookCommand { get; }
        public ICommand UpdateBookCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public BookstoreViewModel()
        {
            _dbContext = new BookstoreDbContext();

            Books = new ObservableCollection<Book>(_dbContext.Books.ToList());

            AddBookCommand = new RelayCommand(AddBook);
            UpdateBookCommand = new RelayCommand(UpdateBook);
            DeleteBookCommand = new RelayCommand(DeleteBook);

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                Books = new ObservableCollection<Book>(_dbContext.Books.Include(b => b.BookLanguage).ToList());
                Stores = new ObservableCollection<Store>(_dbContext.Stores.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured, the data could not be loaded: {ex.Message}");
            }
        }

        public void AddBook()
        {
            var newBook = new Book
            {
                ISBN13 = "978-0-1234-5678-9",
                BookTitle = "New Book Title",
                BookPrice = 9.99M,
                BookRelease = DateTime.Now,
                BookLanguage = _dbContext.BookLanguages.FirstOrDefault(),
                Author = _dbContext.Authors.FirstOrDefault(),
                Publisher = _dbContext.Publishers.FirstOrDefault()
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
