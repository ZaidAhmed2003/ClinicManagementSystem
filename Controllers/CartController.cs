using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.ViewModels.Checkout;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicManagementSystem.Controllers
{

	public class CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
	{
		private readonly ApplicationDbContext _context = context;
		private readonly UserManager<ApplicationUser> _userManager = userManager;

		[HttpPost]
		public async Task<IActionResult> AddToCart(Guid productId, int quantity)
		{
			// Validate input
			if (quantity <= 0)
				return BadRequest("Invalid quantity specified.");

			// Get the current user
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Unauthorized("User not found.");

			// Fetch the product and its inventory
			var product = await _context.Products
				.Include(p => p.Inventory)
				.FirstOrDefaultAsync(p => p.ProductId == productId);

			if (product == null)
				return NotFound("Product not found.");

			var availableInventory = product.Inventory?.Quantity ?? 0;

			// Check if requested quantity is available
			if (availableInventory < quantity)
				return BadRequest($"Only {availableInventory} items are available in stock.");

			// Fetch the user's cart or create a new one if it doesn't exist
			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.FirstOrDefaultAsync(c => c.UserId == user.Id);

			if (cart == null)
			{
				// Create a new cart if it doesn't exist
				cart = new CartModel
				{
					CartId = Guid.NewGuid(),
					UserId = user.Id,
					Total = 0,
					CreatedAt = DateTime.UtcNow,
					CartItems = new List<CartItemModel>()
				};

				// Add the new cart to the context and save it first to ensure CartId is assigned
				await _context.Carts.AddAsync(cart);
				await _context.SaveChangesAsync();
			}

			// Check if the product is already in the cart
			var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
			int existingQuantity = cartItem?.Quantity ?? 0;
			int totalQuantityAfterAddition = existingQuantity + quantity;

			// Ensure total quantity after addition doesn't exceed available inventory
			if (totalQuantityAfterAddition > availableInventory)
				return BadRequest($"Cannot add {quantity} items. Only {availableInventory - existingQuantity} more items can be added.");

			if (cartItem == null)
			{
				// Add new item to the cart
				cartItem = new CartItemModel
				{
					CartItemId = Guid.NewGuid(),
					CartId = cart.CartId,  // Ensure CartId is properly assigned here
					ProductId = productId,
					Quantity = quantity,
					CreatedAt = DateTime.UtcNow
				};

				// Add the new CartItem to the cart
				cart.CartItems.Add(cartItem);
				await _context.CartItems.AddAsync(cartItem);
			}
			else
			{
				// Update the quantity of the existing item
				cartItem.Quantity = totalQuantityAfterAddition;
				_context.Entry(cartItem).State = EntityState.Modified;
			}

			// Recalculate the cart's total
			cart.Total = cart.CartItems.Sum(ci => ci.Quantity * product.Price);
			_context.Entry(cart).State = EntityState.Modified;

			// Save changes and handle exceptions
			try
			{
				await _context.SaveChangesAsync();

				// For non-AJAX form submissions, redirect to the cart page
				if (!Request.Headers["X-Requested-With"].ToString().Equals("XMLHttpRequest"))
				{
					TempData["SuccessMessage"] = "Product added to cart successfully.";
					return RedirectToAction("Index", "Cart");
				}

				// For AJAX calls, return a JSON response
				return Json(new
				{
					success = true,
					total = cart.Total,
					cartItemCount = cart.CartItems.Count
				});
			}
			catch (DbUpdateConcurrencyException ex)
			{
				// In case of concurrency issue, return a conflict response
				return Conflict(new
				{
					message = "Failed to update cart. Please try again.",
					details = ex.Message
				});
			}
		}


		// Get Cart
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Json(new { success = false, message = "User not logged in" });

			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.ThenInclude(p => p.Discount) // Include product discount details
				.FirstOrDefaultAsync(c => c.UserId == user.Id);

			// If no cart found, return an empty cart
			cart ??= new CartModel { CartItems = [], Total = 0 };

			var subtotal = 0m;
			var shippingCost = 10.00m; // Fixed shipping cost, can be adjusted based on conditions
			var totalDiscount = 0m; // To track the total discount applied on products

			foreach (var item in cart.CartItems)
			{
				var productPrice = item.Product?.Price ?? 0;
				var productDiscount = 0m;

				// Check if the product has a discount
				if (item.Product?.Discount != null)
				{
					// Apply the discount only if it exists
					productDiscount = item.Product.Discount.DiscountValue;

					// If you want to apply the discount to the price
					subtotal += item.Quantity * (productPrice - productDiscount);
					totalDiscount += productDiscount * item.Quantity;
				}
				else
				{
					// No discount, add the product price normally
					subtotal += item.Quantity * productPrice;
				}
			}

			var total = subtotal + shippingCost; // Final total (including shipping cost)

			if (Request.Headers.XRequestedWith == "XMLHttpRequest")
			{
				return Json(new
				{
					success = true,
					cartItems = cart.CartItems.Select(ci => new
					{
						cartItemId = ci.CartItemId,
						product = new
						{
							name = ci.Product?.Name,
							price = ci.Product?.Price,
							discount = ci.Product?.Discount?.DiscountValue ?? 0,
							imagePath = ci.Product?.ImagePath
						},
						quantity = ci.Quantity,
						totalPrice = (ci.Quantity * (ci.Product?.Price ?? 0)) - (ci.Product?.Discount?.DiscountValue ?? 0)
					}),
					subtotal,
					discountAmount = totalDiscount,
					shippingCost,
					total
				});
			}

			return View(cart);
		}

		// Remove Item From Cart
		[HttpPost]
		public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Json(new { success = false, message = "User not logged in" });

			var cartItem = await _context.CartItems
				.Include(ci => ci.Cart)
				.ThenInclude(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == user.Id);

			if (cartItem == null) return Json(new { success = false, message = "Cart item not found" });

			_context.CartItems.Remove(cartItem);
			await _context.SaveChangesAsync();

			var newTotal = cartItem.Cart?.CartItems
				.Where(ci => ci.Product != null)
				.Sum(ci => ci.Quantity * ci.Product.Price) ?? 0;

			return Json(new { success = true, total = newTotal });
		}

		// Increment Cart Item
		[HttpPost]
		public async Task<IActionResult> IncrementCartItem(Guid cartItemId)
		{
			// Get the current user
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Json(new { success = false, message = "User not logged in" });

			// Fetch the cart item along with the associated product and inventory
			var cartItem = await _context.CartItems
				.Include(ci => ci.Cart)
				.ThenInclude(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.ThenInclude(p => p.Inventory)  // Ensure we load the inventory as well
				.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == user.Id);

			if (cartItem == null)
				return Json(new { success = false, message = "Cart item not found" });

			var product = cartItem.Product;
			var availableInventory = product?.Inventory?.Quantity ?? 0;

			// Debugging: Log the values to ensure they are correct
			Console.WriteLine($"Cart Item Quantity: {cartItem.Quantity}");
			Console.WriteLine($"Available Inventory: {availableInventory}");

			// Check if incrementing the quantity exceeds the available inventory
			if (cartItem.Quantity + 1 > availableInventory)
			{
				return Json(new { success = false, message = $"Only {availableInventory - cartItem.Quantity} items are available." });
			}

			// Increment the quantity of the cart item
			cartItem.Quantity++;

			// Save the updated cart item
			_context.CartItems.Update(cartItem);

			// Recalculate the cart total
			var newTotal = cartItem.Cart?.CartItems
				.Where(ci => ci.Product != null)
				.Sum(ci => ci.Quantity * ci.Product.Price) ?? 0;

			try
			{
				// Save changes to the database
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				// Handle any errors that might occur during the save operation
				return Json(new { success = false, message = "An error occurred while updating the cart item.", error = ex.Message });
			}

			// Return the updated cart item and total price
			return Json(new { success = true, quantity = cartItem.Quantity, total = newTotal });
		}



		// Decrement Cart Item
		[HttpPost]
		public async Task<IActionResult> DecrementCartItem(Guid cartItemId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Json(new { success = false, message = "User not logged in" });

			// Fetch the cart item with product and inventory details
			var cartItem = await _context.CartItems
				.Include(ci => ci.Cart)
				.ThenInclude(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == user.Id);

			if (cartItem == null)
				return Json(new { success = false, message = "Cart item not found" });

			// Prevent decrementing quantity below 1
			if (cartItem.Quantity <= 1)
			{
				return Json(new { success = false, message = "Quantity cannot be less than 1" });
			}

			// Decrement the quantity of the cart item
			cartItem.Quantity--;
			_context.CartItems.Update(cartItem);
			await _context.SaveChangesAsync();

			// Recalculate the cart's total
			var newTotal = cartItem.Cart?.CartItems
				.Where(ci => ci.Product != null)
				.Sum(ci => ci.Quantity * ci.Product.Price) ?? 0;

			return Json(new { success = true, quantity = cartItem.Quantity, total = newTotal });
		}

		// Get Cart Count 
		[HttpGet]
		public async Task<IActionResult> GetCartItemCount()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Json(new { count = 0 });

			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.FirstOrDefaultAsync(c => c.UserId == user.Id);

			int itemCount = cart?.CartItems.Sum(ci => ci.Quantity) ?? 0;

			return Json(new { count = itemCount });
		}


		public IActionResult ProceedToCheckout()
		{
			return RedirectToAction("Index", "Checkout");
		}

	}
}
