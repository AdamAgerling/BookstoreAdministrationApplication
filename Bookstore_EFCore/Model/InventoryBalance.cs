using Microsoft.EntityFrameworkCore;

namespace BookstoreAdmin.Model
{
    [PrimaryKey(nameof(StoreId), nameof(ISBN13))]
    public class InventoryBalance
    {
        public int StoreId { get; set; }
        public Store Store { get; set; }

        public string ISBN13 { get; set; }
        public Book Book { get; set; }

        public int BookQuantity { get; set; }
    }
}