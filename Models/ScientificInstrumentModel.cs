using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class ScientificInstrumentModel
    {
        [Key]
        public Guid InstrumentId { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [Required, MaxLength(50)]
        public required string Category { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public required string Description { get; set; }

        public int Stock { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
