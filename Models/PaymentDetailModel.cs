using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
    public class PaymentDetailModel
    {
		[Key]
		public Guid PaymentId { get; set; }

		[Required]
		public required string Id { get; set; }

		[ForeignKey("Id")]
		public required ApplicationUser User { get; set; }

		[Required, DataType(DataType.Currency)]
		public decimal Amount { get; set; }

		[Required]
		public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

		[Required]
		public required string PaymentMethod { get; set; }

		[Required]
		public PaymentStatus Status { get; set; } // Using enum for status
	}

	public enum PaymentStatus
	{
		Pending,
		Completed,
		Failed,
		Refunded
	}
}

