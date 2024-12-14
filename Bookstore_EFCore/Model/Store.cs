using BookstoreAdmin.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace BookstoreAdmin.Model
{
    public class Store : BaseViewModel
    {
        private string _storeName;
        private string _storeStreetAddress;


        [Key]
        public int StoreId { get; set; }
        public string StoreName
        {
            get => _storeName;
            set
            {
                _storeName = value;
                OnPropertyChanged(nameof(StoreName));
            }
        }

        public string StoreStreetAddress
        {
            get => _storeStreetAddress;
            set
            {
                _storeStreetAddress = value;
                OnPropertyChanged(nameof(StoreStreetAddress));
            }
        }


        public string StoreNameAndStreet => $"{StoreName}, {StoreStreetAddress}";
        public ICollection<InventoryBalance>? InventoryBalances { get; set; }
        public List<PurchaseHistory>? PurchaseHistories { get; set; }
    }
}