using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class MedicineModel
    {
        [Key]
        public Guid MedicineId { get; set; }

        [Required, MaxLength(50)]
        public required string Code { get; set; }


        [Required]
        public required string Name { get; set; }

        [Required, MaxLength(50)]
        public required string Dosage { get; set; }

        public required string Purpose { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
