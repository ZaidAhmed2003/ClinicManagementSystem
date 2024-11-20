using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.ViewModels.ProfileManagement
{
	public class EditProfileViewModel
	{
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
		public string City { get; set; }
		public string Mobile { get; set; }
	}
}
