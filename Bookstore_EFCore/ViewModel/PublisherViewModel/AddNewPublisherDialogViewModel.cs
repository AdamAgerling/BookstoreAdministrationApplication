using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel.PublisherViewModel
{
    public class AddNewPublisherDialogViewModel : BaseViewModel
    {
        private readonly TaskCompletionSource<Publisher> _tcs;
        private string _newPublisherName;
        private string _newPublisherCountry;
        private int? _newPublisherFoundedYear;

        public string NewPublisherName
        {
            get => _newPublisherName;
            set
            {
                _newPublisherName = value;
                OnPropertyChanged(nameof(NewPublisherName));
            }
        }

        public string NewPublisherCountry
        {
            get => _newPublisherCountry;
            set
            {
                _newPublisherCountry = value;
                OnPropertyChanged(nameof(NewPublisherCountry));
            }
        }

        public int? NewPublisherFoundedYear
        {
            get => _newPublisherFoundedYear;
            set
            {
                _newPublisherFoundedYear = value;
                OnPropertyChanged(nameof(NewPublisherFoundedYear));

            }
        }
        public event Action<Publisher> PublisherCreated;

        public ICommand CreatePublisherCommand { get; }

        public AddNewPublisherDialogViewModel(TaskCompletionSource<Publisher> tcs)
        {
            _tcs = tcs;

            CreatePublisherCommand = new RelayCommand(CreatePublisher);
        }
        private async void CreatePublisher()
        {
            try
            {
                if (!string.IsNullOrEmpty(NewPublisherName) &&
                    !string.IsNullOrEmpty(NewPublisherCountry)
                   )
                {
                    using (var db = new BookstoreDbContext())
                    {
                        var existingPublisher = db.Publishers.FirstOrDefault(p =>
                            p.PublisherName == NewPublisherName &&
                            p.PublisherCountry == NewPublisherCountry &&
                            p.PublisherFoundedYear == NewPublisherFoundedYear);

                        if (existingPublisher != null)
                        {
                            Debug.WriteLine("An publisher with the same name and birth date already exists. Aborting creation.");
                            return;
                        }

                        var newPublisher = new Publisher
                        {
                            PublisherName = NewPublisherName,
                            PublisherCountry = NewPublisherCountry,
                            PublisherFoundedYear = NewPublisherFoundedYear,

                        };

                        db.Publishers.Add(newPublisher);
                        await db.SaveChangesAsync();

                        Debug.WriteLine($"New publisher added successfully: {newPublisher.PublisherName}");

                        PublisherCreated?.Invoke(newPublisher);
                        _tcs.SetResult(newPublisher);
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
                Debug.WriteLine($"Error in CreatePublisher: {ex.Message}");
                MessageBox.Show($"An error occurred while creating the publisher: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

