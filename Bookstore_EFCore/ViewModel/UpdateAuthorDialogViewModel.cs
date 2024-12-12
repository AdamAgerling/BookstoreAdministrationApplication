using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel
{
    internal class UpdateAuthorDialogViewModel : BaseViewModel
    {
        private readonly TaskCompletionSource<Author> _tcs;
        public int AuthorId { get; }
        public string NewAuthorName { get; set; }
        public string NewAuthorLastName { get; set; }
        public string NewAuthorBirthCountry { get; set; }
        public DateTime? NewAuthorBirthDate { get; set; }
        public DateTime? NewAuthorDeathDate { get; set; }

        public ICommand UpdateAuthorCommand { get; }

        public UpdateAuthorDialogViewModel(Author selectedAuthor,
            TaskCompletionSource<Author> tcs)
        {
            _tcs = tcs;

            AuthorId = selectedAuthor.AuthorId;
            NewAuthorName = selectedAuthor.AuthorName;
            NewAuthorLastName = selectedAuthor.AuthorLastName;
            NewAuthorBirthCountry = selectedAuthor.AuthorBirthCountry;
            NewAuthorBirthDate = selectedAuthor.AuthorBirthDate;
            NewAuthorDeathDate = selectedAuthor.AuthorDeathDate;

            UpdateAuthorCommand = new RelayCommand<Window>(UpdateAuthor);
        }

        private void UpdateAuthor(Window window)
        {
            if (string.IsNullOrWhiteSpace(NewAuthorName) || NewAuthorLastName == null || NewAuthorBirthCountry == null || NewAuthorBirthDate == null)
            {
                MessageBox.Show("All required fields must be filled to update the author.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewAuthorDeathDate.HasValue && NewAuthorDeathDate < NewAuthorBirthDate)
            {
                MessageBox.Show("Death date cannot be earlier than birth date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updatedAuthor = new Author
            {
                AuthorId = AuthorId,
                AuthorName = NewAuthorName,
                AuthorLastName = NewAuthorLastName,
                AuthorBirthCountry = NewAuthorBirthCountry,
                AuthorBirthDate = NewAuthorBirthDate,
                AuthorDeathDate = NewAuthorDeathDate,
            };

            if (window != null)
            {
                _tcs.SetResult(updatedAuthor);
                window.DialogResult = true;
            }
            else
            {
                MessageBox.Show("An unexpected error occurred: Unable to close the dialog.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

