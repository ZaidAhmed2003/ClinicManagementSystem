using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords don't match.")]
		public string ConfirmPassword { get; set; } = string.Empty;

		[Required]
		[StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
		public string LastName { get; set; } = string.Empty;

	}
}
