using BookstoreAdmin.Model;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookstoreAdmin.ViewModel.BookViewModel
{
    class ManageStockDialogViewModel : BaseViewModel
    {
        private readonly TaskCompletionSource<InventoryBalance> _tcs;
        private readonly string _isbn13;
        private readonly bool _isRemovingStock;
        public string DialogTitle { get; }
        public string DialogButtonText { get; }
        public ObservableCollection<Store> Stores { get; }

        public Store SelectedStore { get; set; }
        public int BookQuantity { get; set; }

        public ICommand UpdateStockCommand { get; }

        public ManageStockDialogViewModel(ObservableCollection<Store> stores, string isbn13, TaskCompletionSource<InventoryBalance> tcs, bool isRemovingStock = false)
        {
            Stores = stores;
            _isbn13 = isbn13;
            _tcs = tcs;
            _isRemovingStock = isRemovingStock;

            UpdateStockCommand = new RelayCommand<Window>(UpdateStock);

            DialogTitle = isRemovingStock ? "Remove from Stock" : "Add to Stock";
            DialogButtonText = isRemovingStock ? "Remove" : "Add";

        }

        private void UpdateStock(Window window)
        {
            if (SelectedStore == null || BookQuantity <= 0)
            {
                MessageBox.Show("Please select a store and enter a valid quantity", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var inventoryBalance = new InventoryBalance
            {
                StoreId = SelectedStore.StoreId,
                ISBN13 = _isbn13,
                BookQuantity = _isRemovingStock ? -BookQuantity : BookQuantity
            };

            if (window != null)
            {
                _tcs.SetResult(inventoryBalance);
                window.DialogResult = true;
            }
        }
    }
}
