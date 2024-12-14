using BookstoreAdmin.Dialog.Store;
using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel.StoreViewModel
{
    class StoresViewModel : BaseViewModel
    {
        private readonly BookstoreDbContext _dbContext;
        private Store _selectedStore;
        private string _newStoreName;
        private string _newStoreStreetAddress;
        private ObservableCollection<InventoryBalance> _selectedStoreInventory;

        public ObservableCollection<InventoryBalance> SelectedStoreInventory
        {
            get => _selectedStoreInventory;
            set
            {
                _selectedStoreInventory = value;
                OnPropertyChanged();
            }
        }

        public string NewStoreName
        {
            get => _newStoreName;
            set
            {
                _newStoreName = value;
                OnPropertyChanged(nameof(NewStoreName));
            }
        }

        public int StoreId { get; set; }
        public string NewStoreStreetAddress
        {
            get => _newStoreStreetAddress;
            set
            {
                _newStoreStreetAddress = value;
                OnPropertyChanged(nameof(NewStoreStreetAddress));
            }
        }

        private ObservableCollection<InventoryBalance> _inventoryBalances;

        public ObservableCollection<InventoryBalance> InventoryBalances
        {
            get => _inventoryBalances;
            set
            {
                _inventoryBalances = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Store> Stores { get; set; }

        public Store SelectedStore
        {
            get => _selectedStore;
            set
            {
                _selectedStore = value;
                OnPropertyChanged(nameof(SelectedStore));
                (DeleteStoreCommand as RelayCommand)?.NotifyCanExecuteChanged();
                UpdateSelectedStoreInventory();
            }
        }

        public ICommand DeleteStoreCommand { get; }
        public ICommand OpenAddStoreDialogCommand { get; }
        public ICommand OpenUpdateStoreDialogCommand { get; }


        public StoresViewModel()
        {
            _dbContext = new BookstoreDbContext();

            LoadStores();

            DeleteStoreCommand = new RelayCommand(DeleteStore, CanDeleteStore);
            OpenAddStoreDialogCommand = new AsyncRelayCommand(OpenAddStoreDialogAsync);
            OpenUpdateStoreDialogCommand = new AsyncRelayCommand(OpenUpdateStoreDialogAsync);
        }

        private void UpdateSelectedStoreInventory()
        {
            if (SelectedStore == null)
            {
                SelectedStoreInventory = new ObservableCollection<InventoryBalance>();
                return;
            }

            SelectedStoreInventory = new ObservableCollection<InventoryBalance>(
                _dbContext.InventoryBalances
                    .AsNoTracking()
                    .Where(ib => ib.StoreId == SelectedStore.StoreId)
                    .Include(ib => ib.Book)
                    .ThenInclude(b => b.Author)
                    .ToList()
            );
        }

        public void LoadStores()
        {
            Stores = new ObservableCollection<Store>(
                _dbContext.Stores
                .AsNoTracking()
                .Include(s => s.InventoryBalances)
                .ThenInclude(b => b.Book)
                .ThenInclude(a => a.Author)
                .ToList());
        }

        private async Task OpenAddStoreDialogAsync()
        {
            var tcs = new TaskCompletionSource<Store>();
            var dialogViewModel = new AddNewStoreDialogViewModel(tcs);

            dialogViewModel.StoreCreated += store =>
            {
                Stores.Add(store);
                SelectedStore = store;
            };

            var dialog = new AddNewStoreDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                Debug.WriteLine("Store dialog returned true.");
            }
            else
            {
                Debug.WriteLine("Store dialog was canceled.");
            }
        }

        public async Task OpenUpdateStoreDialogAsync()
        {
            if (SelectedStore == null)
            {
                MessageBox.Show("Please select a store to update.", "No store selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tcs = new TaskCompletionSource<Store>();
            var dialogViewModel = new UpdateStoreDialogViewModel(SelectedStore, tcs);

            var dialog = new UpdateStoreDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                var updatedStore = await tcs.Task;
                if (updatedStore != null)
                    try
                    {
                        _dbContext.Entry(SelectedStore).State = EntityState.Detached;

                        var storeToUpdate = _dbContext.Stores.FirstOrDefault(s => s.StoreId == updatedStore.StoreId);

                        if (storeToUpdate != null)
                        {
                            storeToUpdate.StoreName = updatedStore.StoreName;
                            storeToUpdate.StoreStreetAddress = updatedStore.StoreStreetAddress;

                            await _dbContext.SaveChangesAsync();

                            MessageBox.Show($"The store \"{SelectedStore.StoreName}\" has been updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            var index = Stores.IndexOf(SelectedStore);
                            if (index >= 0)
                            {
                                Stores[index] = null;
                                Stores[index] = storeToUpdate;
                                SelectedStore = storeToUpdate;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while updating the store: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
            }
            else
            {
                Debug.WriteLine("Update dialog was canceled.");
            }
        }

        private void DeleteStore()
        {
            if (SelectedStore == null)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the store '{SelectedStore.StoreName}'?",
                "Delete Store",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbContext.Entry(SelectedStore).State = EntityState.Detached;

                    var storeToDelete = _dbContext.Stores.Include(s => s.InventoryBalances).FirstOrDefault(s => s.StoreId == SelectedStore.StoreId);

                    if (storeToDelete != null)
                    {
                        _dbContext.Stores.Remove(storeToDelete);
                        _dbContext.SaveChanges();

                        Stores.Remove(SelectedStore);
                        SelectedStore = null;

                        MessageBox.Show("Store deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the store: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private bool CanDeleteStore()
        {
            return SelectedStore != null;
        }

    }
}

