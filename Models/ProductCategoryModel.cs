using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class ProductCategoryModel
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

		public DateTime? DeletedAt { get; set; } = null;
    }
}
