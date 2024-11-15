using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class ProductInventoryModel
    {
		[Key]
		public Guid InventoryId { get; set; } // Primary key

		[Required]
		public int Quantity { get; set; } = 0; // Required quantity, defaults to 0

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC

		public DateTime ModifiedAt { get; set; } = DateTime.UtcNow; // Tracks last modification
	}
}
