using BookstoreAdmin.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAdmin.Model
{
    [PrimaryKey(nameof(StoreId), nameof(ISBN13))]
    public class InventoryBalance : BaseViewModel
    {
        public int StoreId { get; set; }
        public Store Store { get; set; }

        public string ISBN13 { get; set; }
        public Book Book { get; set; }

        public int BookQuantity { get; set; }
        public decimal InventoryValue => BookQuantity * (Book?.BookPrice ?? 0);
    }
}