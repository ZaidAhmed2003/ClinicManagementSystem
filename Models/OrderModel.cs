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
		public required string Id { get; set; }

		[ForeignKey("Id")]
		public required ApplicationUser User { get; set; }

		[Required]
		public DateTime OrderDate { get; set; } = DateTime.UtcNow;

		[Required]
		public Guid PaymentId { get; set; }

		[ForeignKey("PaymentId")]
		public required PaymentDetailModel PaymentDetail { get; set; }


		[Required, DataType(DataType.Currency)]
		public decimal TotalAmount { get; set; }

		[Required]
		public OrderStatus Status { get; set; } // Enum representing order status
	}
	public enum OrderStatus
	{
		Pending,
		Processing,
		Completed,
		Canceled
	}
}
