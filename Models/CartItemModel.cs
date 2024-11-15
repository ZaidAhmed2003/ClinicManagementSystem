using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class CartItemModel
    {
		[Key]
		public Guid CartItemId { get; set; }

		[Required]
		public Guid CartId { get; set; }

		[ForeignKey("CartId")]
		public required CartModel Cart { get; set; }

		[Required]
		public Guid ProductId { get; set; }

		[ForeignKey("ProductId")]
		public required ProductModel Product { get; set; }

		[Required]
		public int Quantity { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
