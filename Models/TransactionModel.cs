using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class TransactionModel
	{
		[Key]
		public Guid TransactionId { get; set; }

		[Required]
		public Guid UserId { get; set; } // Match the primary key type in ApplicationUser
		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; } = null!; // Navigation property

		[Required]
		public Guid OrderId { get; set; }
		[ForeignKey("OrderId")]
		public required OrderModel Order { get; set; }

		[Required]
		public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

		[Required, DataType(DataType.Currency)]
		public decimal Amount { get; set; }

		[Required]
		public TransactionStatus TransactionStatus { get; set; } // Using enum for transaction status
	}

	public enum TransactionStatus
	{
		Pending,
		Success,
		Failed,
		Reversed,
		Canceled
	}
}

