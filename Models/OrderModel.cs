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
		public Guid UserId { get; set; }

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; } = null!;

		[Required]
		public DateTime OrderDate { get; set; } = DateTime.UtcNow;

		[Required, DataType(DataType.Currency)]
		public decimal TotalAmount { get; set; }

		[Required]
		public OrderStatus Status { get; set; }

		public ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
	}


	public enum OrderStatus
	{
		Pending,
		Processing,
		Completed,
		Canceled
	}
}
