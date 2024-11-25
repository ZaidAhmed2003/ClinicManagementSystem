using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
	public class OrderModel
	{
		[Key]
		public Guid OrderId { get; set; }

		[Required]
		public required string TrackingID { get; set; }

		[Required]
		public Guid PaymentId { get; set; }
		[ForeignKey("PaymentId")]
		public required PaymentDetailModel PaymentDetail { get; set; }

		[Required]
		public Guid UserId { get; set; } // Match the primary key type in ApplicationUser

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; } = null!; // Navigation property

		[Required]
		public DateTime OrderDate { get; set; } = DateTime.UtcNow;

		[Required, DataType(DataType.Currency)]
		public decimal TotalAmount { get; set; }

		[Required]
		public OrderStatus Status { get; set; } // Enum representing order status

		// Add the navigation property for OrderItems
		public ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>(); // This is the missing collection
	}

	public enum OrderStatus
	{
		Pending,
		Processing,
		Completed,
		Canceled
	}
}
