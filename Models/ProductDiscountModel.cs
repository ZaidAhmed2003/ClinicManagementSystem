using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class ProductDiscountModel
    {
        [Key]
        public Guid DiscountId { get; set; }

        public required string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be positive.")]
        public decimal DiscountValue { get; set; } // Discount value (e.g., 10% or $10)

        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; } = null;
    }
}
