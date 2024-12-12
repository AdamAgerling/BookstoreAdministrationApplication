using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel
{
    class AddNewAuthorDialogViewModel : BaseViewModel
    {
        private readonly TaskCompletionSource<Author> _tcs;
        private string _newAuthorName;
        private string _newAuthorLastName;
        private string _newAuthorBirthCountry;
        private DateTime? _newAuthorBirthDate;
        private DateTime? _newAuthorDeatDate;


        public string NewAuthorName
        {
            get => _newAuthorName;
            set
            {
                _newAuthorName = value;
                OnPropertyChanged(nameof(NewAuthorName));
            }
        }
        public string NewAuthorLastName
        {
            get => _newAuthorLastName;
            set
            {
                _newAuthorLastName = value;
                OnPropertyChanged(nameof(NewAuthorLastName));
            }
        }

        public string NewAuthorBirthCountry
        {
            get => _newAuthorBirthCountry;
            set
            {
                _newAuthorBirthCountry = value;
                OnPropertyChanged(nameof(NewAuthorBirthCountry));
            }
        }

        public DateTime? NewAuthorBirthDate
        {
            get => _newAuthorBirthDate;
            set
            {
                _newAuthorBirthDate = value;
                OnPropertyChanged(nameof(NewAuthorBirthDate));

            }
        }

        public DateTime? NewAuthorDeathDate
        {
            get => _newAuthorDeatDate;
            set
            {
                _newAuthorDeatDate = value;
                OnPropertyChanged(nameof(NewAuthorDeathDate));
            }
        }
        public event Action<Author> AuthorCreated;

        public ICommand CreateAuthorCommand { get; }


        public AddNewAuthorDialogViewModel(TaskCompletionSource<Author> tcs)
        {
            _tcs = tcs;

            CreateAuthorCommand = new RelayCommand(CreateAuthor);
        }

        private async void CreateAuthor()
        {
            try
            {
                if (!string.IsNullOrEmpty(NewAuthorName) &&
                    !string.IsNullOrEmpty(NewAuthorLastName) &&
                    !string.IsNullOrEmpty(NewAuthorBirthCountry) &&
                    NewAuthorBirthDate != null)
                {
                    using (var db = new BookstoreDbContext())
                    {
                        var existingAuthor = db.Authors.FirstOrDefault(a =>
                            a.AuthorName == NewAuthorName &&
                            a.AuthorLastName == NewAuthorLastName &&
                            a.AuthorBirthDate == NewAuthorBirthDate);

                        if (existingAuthor != null)
                        {
                            Debug.WriteLine("An author with the same name and birth date already exists. Aborting creation.");
                            return;
                        }

                        var newAuthor = new Author
                        {
                            AuthorName = NewAuthorName,
                            AuthorLastName = NewAuthorLastName,
                            AuthorBirthCountry = NewAuthorBirthCountry,
                            AuthorBirthDate = NewAuthorBirthDate.Value,
                            AuthorDeathDate = NewAuthorDeathDate
                        };

                        db.Authors.Add(newAuthor);
                        await db.SaveChangesAsync();

                        Debug.WriteLine($"New author added successfully: {newAuthor.AuthorName} {newAuthor.AuthorLastName}");

                        AuthorCreated?.Invoke(newAuthor);
                        _tcs.SetResult(newAuthor);
                        Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                    }
                }
                else
                {
                    Debug.WriteLine("Validation failed. Inputs are not valid.");
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateAuthor: {ex.Message}");
                MessageBox.Show($"An error occurred while creating the author: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
