using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreAdmin.Model
{
    public class PurchaseHistory
    {
        [Key]
        public int PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PurchaseQuantity { get; set; }
        public decimal TotalPrice { get; set; }

        [ForeignKey("Store")]
        public int StoreId { get; set; }
        public required Store Store { get; set; }

        [ForeignKey("Book")]
        public required string ISBN13 { get; set; }
        public required Book Book { get; set; }
    }
}