using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class NotificationModel
	{
		[Key]
		public Guid NotificationId { get; set; }

		[Required]
		public Guid UserId { get; set; } // Match the primary key type in ApplicationUser

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; } = null!; // Navigation property

		[Required]
		public string? Title { get; set; }

		public string? Message { get; set; }

		[Required]
		public bool IsRead { get; set; }

		[Required]
		public DateTime CreateTime { get; set; } = DateTime.UtcNow;

		public DateTime? ExpiryDate { get; set; }
	}
}
