using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicManagementSystem.ViewModels.ProductsManagement
{
	public class ProductIndexViewModel
	{
		public IEnumerable<ProductModel> Products { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public string SearchTerm { get; set; }
		public string CategoryFilter { get; set; }
		public string SortBy { get; set; }
		public IEnumerable<SelectListItem> Categories { get; set; }
	}


}
