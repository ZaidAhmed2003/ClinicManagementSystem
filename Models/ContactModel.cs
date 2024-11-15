using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class ContactModel
	{
		[Key]
		public Guid ContactId { get; set; }

		[Required]
		public required string Id { get; set; }

		[ForeignKey("Id")]
		public required ApplicationUser User { get; set; }

		[Required]
		public required string Name { get; set; }

		[Required, EmailAddress]
		public required string Email { get; set; }

		public required string Subject { get; set; }

		public required string Message { get; set; }

		public bool IsMarkedRead { get; set; } =false;

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedAt { get; set; }
	}
}
