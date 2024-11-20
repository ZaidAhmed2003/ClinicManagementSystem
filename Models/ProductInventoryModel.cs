using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
    public class ProductInventoryModel
    {
		[Key]
		public Guid InventoryId { get; set; } // Primary key

		[Required]
		[Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
		public int Quantity { get; set; } = 0;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC

		public DateTime ModifiedAt { get; set; } = DateTime.UtcNow; // Tracks last modification
	}
}
