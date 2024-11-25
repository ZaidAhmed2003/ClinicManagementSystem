using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
	public class OrderItemModel
	{
		[Key]
		public Guid OrderItemId { get; set; }

		public Guid ProductId { get; set; }
		[ForeignKey("ProductId")]
		public required ProductModel Product { get; set; }

		public Guid OrderId { get; set; }
		[ForeignKey("OrderId")]
		public required OrderModel Order { get; set; }

		[Required, DataType(DataType.Currency)]
		[Range(0.01, 1000000)]
		public decimal Price { get; set; }

		[Required]
		public int Quantity { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}

}
