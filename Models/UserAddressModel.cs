using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
	public class UserAddressModel
	{
		[Key]
		public Guid AddressId { get; set; }

		[Required]
		public Guid UserId { get; set; } // Match the primary key type in ApplicationUser

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; } = null!; // Navigation property

		[Required]
		public required string AddressLine1 { get; set; }

		public string? AddressLine2 { get; set; }

		[Required]
		public string PostalCode { get; set; } = string.Empty;

		[Required]
		public required string Country { get; set; }

		[Required]
		public required string City { get; set; }

		public string? Telephone { get; set; }

		[Required]
		public required string Mobile { get; set; }
	}
}
