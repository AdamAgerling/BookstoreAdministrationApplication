using BookstoreAdmin.Dialog.Author;
using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel.AuthorViewModel
{
    internal class AuthorsViewModel : BaseViewModel
    {
        private readonly BookstoreDbContext _dbContext;
        private Author _selectedAuthor;
        private ObservableCollection<Book> _selectedAuthorBooks;

        public string DeathDateText => SelectedAuthor != null && SelectedAuthor.AuthorDeathDate.HasValue ? "Died: " : string.Empty;
        public string AuthorAgeDisplay
        {
            get
            {
                if (SelectedAuthor == null)
                    return string.Empty;

                return SelectedAuthor.AuthorDeathDate.HasValue
                    ? $"{AuthorAge} (Dead)"
                    : $"{AuthorAge}";
            }
        }
        public string BookTitles => SelectedAuthor != null
          ? $"{SelectedAuthor.AuthorName} {SelectedAuthor.AuthorLastName}'s books in our system"
          : "No author selected";

        public int? AuthorAge
        {
            get
            {
                if (SelectedAuthor == null || !SelectedAuthor.AuthorBirthDate.HasValue)
                    return null;

                var birthDate = SelectedAuthor.AuthorBirthDate.Value.Date;
                var deathDate = (SelectedAuthor.AuthorDeathDate ?? DateTime.Now).Date;

                int age = deathDate.Year - birthDate.Year;

                if (birthDate > deathDate.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }

        public ObservableCollection<Author> Authors { get; set; }

        public Author SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged(nameof(SelectedAuthor));
                (DeleteAuthorCommand as RelayCommand)?.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(BookTitles));
                OnPropertyChanged(nameof(AuthorAge));
                OnPropertyChanged(nameof(DeathDateText));
                OnPropertyChanged(nameof(AuthorAgeDisplay));
                UpdateSelectedAuthorBooks();
            }
        }

        public ICommand DeleteAuthorCommand { get; }
        public ICommand OpenAddAuthorDialogCommand { get; }
        public ICommand OpenUpdateAuthorDialogCommand { get; }

        public AuthorsViewModel()
        {
            _dbContext = new BookstoreDbContext();

            LoadAuthors();

            DeleteAuthorCommand = new RelayCommand(DeleteAuthor, CanDeleteAuthor);
            OpenAddAuthorDialogCommand = new AsyncRelayCommand(OpenAddAuthorDialogAsync);
            OpenUpdateAuthorDialogCommand = new AsyncRelayCommand(OpenUpdateAuthorDialogAsync);
        }

        private void UpdateSelectedAuthorBooks()
        {
            if (SelectedAuthor == null)
                return;

            var authorId = SelectedAuthor.AuthorId;

            SelectedAuthor.Books = new ObservableCollection<Book>(
                _dbContext.Books
                    .AsNoTracking()
                    .Where(b => b.AuthorId == authorId)
                    .Include(b => b.Publisher)
                    .ToList());
        }

        public void LoadAuthors()
        {
            Authors = new ObservableCollection<Author>(
              _dbContext.Authors
              .AsNoTracking()
              .Include(a => a.Books)
              .ThenInclude(b => b.Publisher)
              .OrderBy(a => a.AuthorName).ToList());
        }

        private async Task OpenAddAuthorDialogAsync()
        {
            var tcs = new TaskCompletionSource<Author>();
            var dialogViewModel = new AddNewAuthorDialogViewModel(tcs, _dbContext);

            dialogViewModel.AuthorCreated += author =>
            {
                Authors.Add(author);
                SelectedAuthor = author;
            };

            var dialog = new AddNewAuthorDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                Debug.WriteLine("Author dialog returned true.");
            }
            else
            {
                Debug.WriteLine("Author dialog was canceled.");
            }
        }

        private void DeleteAuthor()
        {
            if (SelectedAuthor == null)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the Author '{SelectedAuthor.AuthorName} {SelectedAuthor.AuthorLastName}'?",
                "Delete Author",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbContext.Entry(SelectedAuthor).State = EntityState.Detached;

                    var authorToDelete = _dbContext.Authors.Include(a => a.Books).FirstOrDefault(a => a.AuthorId == SelectedAuthor.AuthorId);

                    if (authorToDelete != null)
                    {
                        _dbContext.Authors.Remove(authorToDelete);
                        _dbContext.SaveChanges();

                        Authors.Remove(SelectedAuthor);
                        SelectedAuthor = null;

                        MessageBox.Show("Author deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the author: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public async Task OpenUpdateAuthorDialogAsync()
        {
            if (SelectedAuthor == null)
            {
                MessageBox.Show("Please select an author to update.", "No author selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tcs = new TaskCompletionSource<Author>();
            var dialogViewModel = new UpdateAuthorDialogViewModel(SelectedAuthor, tcs);

            var dialog = new UpdateAuthorDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                var updatedAuthor = await tcs.Task;
                if (updatedAuthor != null)
                {
                    try
                    {
                        _dbContext.Entry(SelectedAuthor).State = EntityState.Detached;

                        var authorToUpdate = _dbContext.Authors.FirstOrDefault(a => a.AuthorId == updatedAuthor.AuthorId);

                        if (authorToUpdate != null)
                        {
                            authorToUpdate.AuthorName = updatedAuthor.AuthorName;
                            authorToUpdate.AuthorLastName = updatedAuthor.AuthorLastName;
                            authorToUpdate.AuthorBirthCountry = updatedAuthor.AuthorBirthCountry;
                            authorToUpdate.AuthorBirthDate = updatedAuthor.AuthorBirthDate;
                            authorToUpdate.AuthorDeathDate = updatedAuthor.AuthorDeathDate;

                            await _dbContext.SaveChangesAsync();

                            MessageBox.Show($"The author \"{authorToUpdate.AuthorName} {authorToUpdate.AuthorLastName}\" has been updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            var index = Authors.IndexOf(SelectedAuthor);
                            if (index >= 0)
                            {
                                Authors[index] = null;
                                Authors[index] = authorToUpdate;
                                SelectedAuthor = authorToUpdate;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while updating the author: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                Debug.WriteLine("Update dialog was canceled.");
            }
        }

        private bool CanDeleteAuthor()
        {
            return SelectedAuthor != null;
        }
    }
}
