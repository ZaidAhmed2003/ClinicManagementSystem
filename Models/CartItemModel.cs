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
		public CartModel Cart { get; set; }  // EF will manage the relationship


		public Guid ProductId { get; set; }
		[ForeignKey("ProductId")]
		public ProductModel Product { get; set; }


		public int Quantity { get; set; } = 1;

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
