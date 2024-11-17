using Microsoft.AspNetCore.Identity;


namespace ClinicManagementSystem.Models
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string? ProfileImagePath { get; set; } = null;
		public DateTime? DateOfBirth { get; set; } = null;

	
	}
}