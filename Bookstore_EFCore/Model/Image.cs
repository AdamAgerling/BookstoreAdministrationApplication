using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreAdmin.Model
{
    [Table("Images")]
    public class Image
    {
        [Key]
        [Column("ImageId")]
        [MaxLength(13)]
        public string Id { get; set; }

        public string ImageUrl { get; set; }
    }
}
