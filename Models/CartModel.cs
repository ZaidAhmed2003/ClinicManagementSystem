using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class CartModel
    {
		[Key]
		public Guid CartId { get; set; }

		[Required]
		public required string Id { get; set; }

		[ForeignKey("Id")]
		public required ApplicationUser User { get; set; }

		[Required, DataType(DataType.Currency)]
		public decimal Total { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow;
	}
}
