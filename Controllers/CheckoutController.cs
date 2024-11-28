using Azure.Core;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.ViewModels.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	[Authorize]
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

		// Place Order
		[HttpPost]
		public async Task<IActionResult> PlaceOrder()
		{
			// Get the logged-in user
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized("User not found.");

			// Fetch the user's cart
			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.ThenInclude(i => i.Inventory)
				.FirstOrDefaultAsync(c => c.UserId == user.Id);

			// Check if the cart is null or empty
			if (cart == null || cart.CartItems.Count == 0)
				return BadRequest("Cart is empty or does not exist.");

			// Calculate total amount
			decimal totalAmount = cart.CartItems.Sum(ci => ci.Product?.Price ?? 0 * ci.Quantity);

			// Create a new PaymentDetail (Simulating payment, replace with actual payment logic)
			var payment = new PaymentDetailModel
			{
				PaymentId = Guid.NewGuid(),
				UserId = user.Id,
				Amount = totalAmount,
				PaymentDate = DateTime.UtcNow,
				PaymentMethod = "Credit Card", // Example, replace with dynamic input
				Status = PaymentStatus.Completed
			};

			await _context.PaymentDetails.AddAsync(payment);

			// Generate the TrackingID using the custom method
			var trackingID = await GenerateNextTrackingId();

			// Create a new Order
			var order = new OrderModel
			{
				OrderId = Guid.NewGuid(),
				TrackingID = trackingID,
				UserId = user.Id,
				PaymentDetail = payment, // Add this line to initialize the required member
				TotalAmount = totalAmount,
				OrderDate = DateTime.UtcNow,
				Status = OrderStatus.Completed
			};

			// Add items to the order and update inventory
			foreach (var cartItem in cart.CartItems)
			{
				var product = cartItem.Product;

				// Null check for product
				if (product == null)
					return BadRequest("Product is missing for one of the cart items.");

				var inventory = product.Inventory;

				// Null check for inventory
				if (inventory == null)
					return BadRequest("Inventory details are missing for the product.");

				// Check if there is enough inventory before adding to the order
				if (inventory.Quantity < cartItem.Quantity)
				{
					return BadRequest($"Not enough stock for product {product.Name}. Only {inventory.Quantity} items available.");
				}

				// Deduct the inventory
				inventory.Quantity -= cartItem.Quantity;
				_context.Product_Inventory.Update(inventory); // Mark inventory as modified

				var orderItem = new OrderItemModel
				{
					OrderItemId = Guid.NewGuid(),
					Order = order, // Initialize the required member
					Product = cartItem.Product, // Initialize the required member
					Quantity = cartItem.Quantity,
					Price = cartItem.Product.Price,
					CreatedAt = DateTime.UtcNow
				};
				order.OrderItems.Add(orderItem);
			}

			await _context.Orders.AddAsync(order);

			// Clear the cart
			_context.CartItems.RemoveRange(cart.CartItems);
			_context.Carts.Remove(cart);

			// Save changes
			try
			{
				await _context.SaveChangesAsync();
				return Json(new { success = true, message = "Order placed successfully.", orderId = order.OrderId });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = "Failed to place order.", details = ex.Message });
			}
		}


		private async Task<string> GenerateNextTrackingId()
		{
			// Fetch the most recent order by order date
			var lastOrder = await _context.Orders
				.OrderByDescending(o => o.OrderDate)
				.FirstOrDefaultAsync();

			var prefix = "CMSO-";
			if (lastOrder?.TrackingID?.StartsWith(prefix) == true)
			{
				// Extract the numeric part and increment it
				var numericPart = lastOrder.TrackingID[prefix.Length..];
				if (int.TryParse(numericPart, out int number))
					return $"{prefix}{(number + 1):D5}";
			}

			// Default to the first TrackingID if no previous order exists
			return $"{prefix}00001";
		}

	}


}
