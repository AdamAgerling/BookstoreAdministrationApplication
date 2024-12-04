using BookstoreAdmin.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace BookstoreAdmin.ViewModel
{
    class BookstoreViewModel
    {
        private BookstoreDbContext _dbContext;

        public ObservableCollection<Book> Books { get; set; }
        public ObservableCollection<Store> Stores { get; set; }

        public BookstoreViewModel()
        {
            _dbContext = new BookstoreDbContext();

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

        public void AddBook(Book newBook)
        {
            try
            {
                _dbContext.Books.Add(newBook);
                _dbContext.SaveChanges();
                Books.Add(newBook);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured, the book could not be added: {ex.Message}");
            }
        }

        public void RemoveBook(Book bookToRemove)
        {
            try
            {
                _dbContext.Books.Remove(bookToRemove);
                _dbContext.SaveChanges();
                Books.Remove(bookToRemove);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured, the book could not be removed: {ex.Message}");
            }
        }

        public void UpdateBook(Book bookToUpdate)
        {
            try
            {
                _dbContext.Books.Update(bookToUpdate);
                _dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured, the book could not be updated: {ex.Message}");
            }
        }
    }
}
