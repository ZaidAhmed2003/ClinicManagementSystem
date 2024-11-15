using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class ProductDiscountModel
    {
        [Key]
        public Guid DiscountId { get; set; }

		[MaxLength(100)]
		public required string Name { get; set; }

		[Required, Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be positive.")]
		public decimal DiscountValue { get; set; } // Required discount value

		public bool IsActive { get; set; } = false; // Indicates if the discount is currently active

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC

		public DateTime ModifiedAt { get; set; } = DateTime.UtcNow; // Tracks last modification

		public DateTime? DeletedAt { get; set; } = null; // Soft delete field
	}
}
