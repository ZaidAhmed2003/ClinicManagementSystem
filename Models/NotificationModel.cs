using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class NotificationModel
	{
		[Key]
		public Guid NotificationId { get; set; }

		[Required]
		public required string Id { get; set; }

		[ForeignKey("Id")]
		public required ApplicationUser User { get; set; }

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
