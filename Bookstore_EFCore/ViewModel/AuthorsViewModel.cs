using BookstoreAdmin.Dialog;
using BookstoreAdmin.Dialog.Author;
using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel
{
    internal class AuthorsViewModel : BaseViewModel
    {
        private readonly BookstoreDbContext _dbContext;
        private Author _selectedAuthor;
        public string DeathDateText => SelectedAuthor != null && SelectedAuthor.AuthorDeathDate.HasValue ? "Death Date: " : string.Empty;

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
            }
        }

        public ObservableCollection<Book> SelectedAuhorBooks => new ObservableCollection<Book>(SelectedAuthor?.Books ?? new List<Book>());
        public ICommand DeleteAuthorCommand { get; }
        public ICommand OpenAddAuthorDialogCommand { get; }
        public ICommand OpenUpdateAuthorDialogCommand { get; }

        public AuthorsViewModel()
        {
            _dbContext = new BookstoreDbContext();

            Authors = new ObservableCollection<Author>(
                _dbContext.Authors.
                Include(a => a.Books).
                ThenInclude(b => b.Publisher).
                OrderBy(a => a.AuthorName).ToList());

            DeleteAuthorCommand = new RelayCommand(DeleteAuthor, CanDeleteAuthor);
            OpenAddAuthorDialogCommand = new AsyncRelayCommand(OpenAddAuthorDialogAsync);
            OpenUpdateAuthorDialogCommand = new AsyncRelayCommand(OpenUpdateAuthorDialogAsync);
        }

        private async Task OpenAddAuthorDialogAsync()
        {
            var tcs = new TaskCompletionSource<Author>();
            var dialogViewModel = new AddNewAuthorDialogViewModel(tcs);

            dialogViewModel.AuthorCreated += author =>
            {
                Authors.Add(author);
                Debug.WriteLine($"Author added to observable collection: {author.AuthorName} {author.AuthorLastName}");
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
                    _dbContext.Authors.Remove(SelectedAuthor);
                    _dbContext.SaveChanges();

                    Authors.Remove(SelectedAuthor);

                    MessageBox.Show("Author deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectedAuthor = null;
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

                        var authorToUpdate = _dbContext.Authors.FirstOrDefault(b => b.AuthorId == updatedAuthor.AuthorId);
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
