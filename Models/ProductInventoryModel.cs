using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class ProductInventoryModel
    {
        [Key]
        public Guid InventoryId { get; set; }

        [Required]
        public int Quantity { get; set; } = 0;


        public DateTime CreatedAt { get; set; }

 
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
