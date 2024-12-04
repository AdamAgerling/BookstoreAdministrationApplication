using System.ComponentModel.DataAnnotations;

namespace BookstoreAdmin.Model
{
    public class Store
    {
        [Key]
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreStreetAddress { get; set; }

        public List<InventoryBalance> InventoryBalances { get; set; }
        public List<PurchaseHistory> PurchaseHistories { get; set; }
    }
}