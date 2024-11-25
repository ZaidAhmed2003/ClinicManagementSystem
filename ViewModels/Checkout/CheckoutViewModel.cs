using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.ViewModels.Checkout
{
	public class CheckoutViewModel
	{
		// CheckoutViewModel.cs
	    public ApplicationUser User { get; set; }
		public CartModel Cart { get; set; }
	    public UserAddressModel UserAddress { get; set; }
		public decimal TotalAmount { get; set; } // This will be calculated at the time of checkout
		public decimal TotalCost { get; internal set; }
		public decimal ShippingCost { get; internal set; }
		public decimal Subtotal { get; internal set; }

		public List<OrderItemModel> OrderItems { get; set; }  // Add this property for the order items
	}

	
}
