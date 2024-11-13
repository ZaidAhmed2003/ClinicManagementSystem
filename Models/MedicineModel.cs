using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class MedicineModel
    {
        [Key]
        public Guid MedicineId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public required string Code { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [Required, MaxLength(50)]
        public required string Brand { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, DataType(DataType.Currency)]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        public bool IsAvailable { get; set; } = false;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? DeletedAt { get; set; }

        public string? ImagePath { get; set; }
    }
}
