using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
    public class PaymentDetailModel
    {
		[Key]
		public Guid PaymentId { get; set; }

		[Required]
		public Guid UserId { get; set; } // Match the primary key type in ApplicationUser

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; } = null!; // Navigation property

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

