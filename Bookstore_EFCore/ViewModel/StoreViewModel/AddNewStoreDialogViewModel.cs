using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel.StoreViewModel
{
    class AddNewStoreDialogViewModel : BaseViewModel
    {
        private readonly TaskCompletionSource<Store> _tcs;
        private string _newStoreName;
        private string _newStoreStreetAddress;

        public string NewStoreName
        {
            get => _newStoreName;
            set
            {
                _newStoreName = value;
                OnPropertyChanged(nameof(NewStoreName));
            }
        }

        public string NewStoreStreetAddress
        {
            get => _newStoreStreetAddress;
            set
            {
                _newStoreStreetAddress = value;
                OnPropertyChanged(nameof(NewStoreStreetAddress));
            }
        }
        public event Action<Store> StoreCreated;

        public ICommand CreateStoreCommand { get; }

        public AddNewStoreDialogViewModel(TaskCompletionSource<Store> tcs)
        {
            _tcs = tcs;

            CreateStoreCommand = new RelayCommand(CreateStore);
        }

        private async void CreateStore()
        {
            try
            {
                if (!string.IsNullOrEmpty(NewStoreName) &&
                    !string.IsNullOrEmpty(NewStoreStreetAddress)
                   )
                {
                    using (var db = new BookstoreDbContext())
                    {
                        var existingStore = db.Stores.FirstOrDefault(s =>
                            s.StoreName == NewStoreName &&
                            s.StoreStreetAddress == NewStoreStreetAddress);

                        if (existingStore != null)
                        {
                            Debug.WriteLine("An store with the same name and street address already exists. Aborting creation.");
                            return;
                        }

                        var newStore = new Store
                        {
                            StoreName = NewStoreName,
                            StoreStreetAddress = NewStoreStreetAddress
                        };

                        db.Stores.Add(newStore);
                        await db.SaveChangesAsync();

                        Debug.WriteLine($"New publisher added successfully: {newStore.StoreName}");

                        StoreCreated?.Invoke(newStore);
                        _tcs.SetResult(newStore);
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
                MessageBox.Show($"An error occurred while creating the store: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

