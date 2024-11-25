using Azure.Core;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.ViewModels.Checkout;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	public class CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
	{
		private readonly ApplicationDbContext _context = context;
		private readonly UserManager<ApplicationUser> _userManager = userManager;

		// Display Checkout Page
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);  // Get the logged-in user

			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.FirstOrDefaultAsync(c => c.UserId == user.Id);

			if (cart == null || cart.CartItems.Count == 0)
			{
				return RedirectToAction("Index", "Home");
			}

			// Calculate the total price including discounts and shipping
			decimal subtotal = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);
			decimal shippingCost = 5.00m;  // Example shipping cost, can be calculated based on location
			decimal total = subtotal + shippingCost;

			var userAddress = await _context.UserAddresses.FirstOrDefaultAsync(ua => ua.UserId == user.Id);

			var checkoutViewModel = new CheckoutViewModel
			{
				User = user,
				Cart = cart,
				UserAddress = userAddress,
				Subtotal = subtotal,
				ShippingCost = shippingCost,
				TotalCost = total
			};

			return View(checkoutViewModel);
		}


		[HttpPost]
		public async Task<IActionResult> PlaceOrder(CheckoutViewModel request)
		{
			var user = await _userManager.GetUserAsync(User);  // Get the logged-in user

			// Ensure OrderItems is not null or empty before proceeding
			if (request.OrderItems == null || !request.OrderItems.Any())
			{
				ModelState.AddModelError("", "Your order does not contain any items.");
				return View(request); // Return to the view with an error
			}

			// Create new Order
			var order = new OrderModel
			{
				OrderId = Guid.NewGuid(),
				TrackingID = Guid.NewGuid().ToString(),
				UserId = user.Id,  // Use logged-in user's Id
				OrderDate = DateTime.UtcNow,
				TotalAmount = request.OrderItems.Sum(item => item.Quantity * item.Price), // Calculate total amount
				Status = OrderStatus.Pending
			};

			// Add the order to the database
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();  // Save the order to the database

			// Add Order Items and update product stock
			foreach (var item in request.OrderItems)
			{
				var product = await _context.Products.FindAsync(item.ProductId);
				if (product == null || product.Inventory.Quantity < item.Quantity)
				{
					ModelState.AddModelError("", $"Product {item.ProductId} is out of stock or does not exist.");
					return View(request); // If product is out of stock, return the view with an error
				}

				// Add the order item
				var orderItem = new OrderItemModel
				{
					OrderItemId = Guid.NewGuid(),
					OrderId = order.OrderId,
					Order = order,  // Add the order here
					ProductId = item.ProductId,
					Price = item.Price,
					Quantity = item.Quantity,
					Product = product  // Set the Product navigation property here
				};

				_context.OrderItems.Add(orderItem);

				// Update the product stock
				product.Inventory.Quantity -= item.Quantity;
				_context.Products.Update(product);
			}

			// Save changes and commit the transaction
			await _context.SaveChangesAsync();

			// Redirect to the order confirmation page
			return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
		}



	}


}
