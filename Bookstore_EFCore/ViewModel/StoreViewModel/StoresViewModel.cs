using BookstoreAdmin.Dialog.Store;
using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using static BookstoreAdmin.ViewModel.StoreViewModel.ManageExistingBookInStoreDialogViewModel;

namespace BookstoreAdmin.ViewModel.StoreViewModel
{
    class StoresViewModel : BaseViewModel
    {
        private readonly BookstoreDbContext _dbContext;
        private Store _selectedStore;
        private string _newStoreName;
        private string _newStoreStreetAddress;
        private ObservableCollection<InventoryBalance> _selectedStoreInventory;

        public string StoreTitles => SelectedStore != null
                ? $"{SelectedStore.StoreNameAndStreet}'s current inventory"
                : "No store selected";

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
                OnPropertyChanged(nameof(StoreTitles));
                (DeleteStoreCommand as RelayCommand)?.NotifyCanExecuteChanged();
                UpdateSelectedStoreInventory();
            }
        }

        public ICommand DeleteStoreCommand { get; }
        public ICommand OpenAddStoreDialogCommand { get; }
        public ICommand OpenUpdateStoreDialogCommand { get; }
        public ICommand OpenAddExistingBookToStoreDialogCommand { get; }
        public ICommand OpenRemoveExistingBookFromStoreDialogCommand { get; }


        public StoresViewModel()
        {
            _dbContext = new BookstoreDbContext();

            LoadStores();

            DeleteStoreCommand = new RelayCommand(DeleteStore, CanDeleteStore);
            OpenAddStoreDialogCommand = new AsyncRelayCommand(OpenAddStoreDialogAsync);
            OpenUpdateStoreDialogCommand = new AsyncRelayCommand(OpenUpdateStoreDialogAsync);
            OpenAddExistingBookToStoreDialogCommand = new AsyncRelayCommand(OpenAddExistingBookToStoreDialogAsync);
            OpenRemoveExistingBookFromStoreDialogCommand = new AsyncRelayCommand(OpenRemoveExistingBookFromStoreDialogAsync);
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

        private async Task OpenAddExistingBookToStoreDialogAsync()
        {
            if (SelectedStore == null)
            {
                MessageBox.Show("Please select a store first.", " No Store Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tcs = new TaskCompletionSource<(Book, int, bool)>();
            var dialogViewModel = new ManageExistingBookInStoreDialogViewModel(new ObservableCollection<Book>(_dbContext.Books), tcs, DialogMode.Add);

            var dialog = new ManageExistingBookInStoreDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                var (selectedBook, quantity, _) = await tcs.Task;

                var inventory = _dbContext.InventoryBalances
                    .FirstOrDefault(ib => ib.StoreId == SelectedStore.StoreId && ib.ISBN13 == selectedBook.ISBN13);

                if (inventory != null)
                {
                    inventory.BookQuantity += quantity;
                }
                else
                {
                    _dbContext.InventoryBalances.Add(new InventoryBalance
                    {
                        StoreId = SelectedStore.StoreId,
                        ISBN13 = selectedBook.ISBN13,
                        BookQuantity = quantity
                    });
                }
                await _dbContext.SaveChangesAsync();
                UpdateSelectedStoreInventory();
                MessageBox.Show("Book added to store invetory.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async Task OpenRemoveExistingBookFromStoreDialogAsync()
        {
            if (SelectedStore == null)
            {
                MessageBox.Show("Please select a store.", "No Store Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tcs = new TaskCompletionSource<(Book, int, bool)>();
            var dialogViewModel = new ManageExistingBookInStoreDialogViewModel(
                new ObservableCollection<Book>(_dbContext.Books.ToList()), tcs, DialogMode.Remove);

            var dialog = new ManageExistingBookInStoreDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                var (selectedBook, quantityToRemove, removeEntirely) = await tcs.Task;

                var inventory = _dbContext.InventoryBalances
                    .FirstOrDefault(ib => ib.StoreId == SelectedStore.StoreId && ib.ISBN13 == selectedBook.ISBN13);

                if (inventory != null)
                {
                    if (removeEntirely)
                    {
                        _dbContext.InventoryBalances.Remove(inventory);
                        MessageBox.Show("The book was completely removed from the store's inventory.", "Removed", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        inventory.BookQuantity -= quantityToRemove;
                        if (inventory.BookQuantity < 0)
                            inventory.BookQuantity = 0;

                        MessageBox.Show("The book quantity was updated.", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    await _dbContext.SaveChangesAsync();
                    UpdateSelectedStoreInventory();
                }
                else
                {
                    MessageBox.Show("This book does not exist in the store's inventory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

