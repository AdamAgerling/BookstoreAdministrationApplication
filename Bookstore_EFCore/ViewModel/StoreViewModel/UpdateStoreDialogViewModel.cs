using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel.StoreViewModel
{
    class UpdateStoreDialogViewModel : BaseViewModel
    {
        private readonly TaskCompletionSource<Store> _tcs;
        public int StoreId { get; }
        public string NewStoreName { get; set; }
        public string NewStoreStreetAddress { get; set; }

        public ICommand UpdateStoreCommand { get; }

        public UpdateStoreDialogViewModel(Store selectedStore,
           TaskCompletionSource<Store> tcs)
        {
            _tcs = tcs;

            StoreId = selectedStore.StoreId;
            NewStoreName = selectedStore.StoreName;
            NewStoreStreetAddress = selectedStore.StoreStreetAddress;

            UpdateStoreCommand = new RelayCommand<Window>(UpdateStore);
        }

        private void UpdateStore(Window window)
        {
            if (string.IsNullOrWhiteSpace(NewStoreName) || NewStoreStreetAddress == null)
            {
                MessageBox.Show("All required fields must be filled to update the store.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updatedStore = new Store
            {
                StoreId = StoreId,
                StoreName = NewStoreName,
                StoreStreetAddress = NewStoreStreetAddress
            };

            if (window != null)
            {
                _tcs.SetResult(updatedStore);
                window.DialogResult = true;
            }
            else
            {
                MessageBox.Show("An unexpected error occurred: Unable to close the dialog.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
