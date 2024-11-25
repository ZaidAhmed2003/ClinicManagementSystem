using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class CartModel
    {
		[Key]
		public Guid CartId { get; set; }

		public Guid UserId { get; set; } // Match the primary key type in ApplicationUser
		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; } = null!; // Navigation property

		[Required, DataType(DataType.Currency)]
		public decimal Total { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? ModifiedAt { get; set; } // Made nullable, only set when updated

		public ICollection<CartItemModel> CartItems { get; set; } = []; // Use ICollection for best practice


	}
}
