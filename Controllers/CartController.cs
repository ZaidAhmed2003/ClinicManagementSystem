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

		// Add To Cart
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

			if (availableInventory < quantity)
				return BadRequest($"Only {availableInventory} items are available in stock.");

			// Fetch the user's cart or create a new one if it doesn't exist
			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.FirstOrDefaultAsync(c => c.UserId == user.Id);

			if (cart == null)
			{
				cart = new CartModel
				{
					CartId = Guid.NewGuid(),
					UserId = user.Id,
					Total = 0,
					CreatedAt = DateTime.UtcNow,
					CartItems = []
				};

				await _context.Carts.AddAsync(cart);
			}

			// Check if the product is already in the cart
			var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
			int existingQuantity = cartItem?.Quantity ?? 0;
			int totalQuantityAfterAddition = existingQuantity + quantity;

			if (totalQuantityAfterAddition > availableInventory)
				return BadRequest($"Cannot add {quantity} items. Only {availableInventory - existingQuantity} more items can be added.");

			if (cartItem == null)
			{
				// Add new item to the cart
				cartItem = new CartItemModel
				{
					CartItemId = Guid.NewGuid(),
					CartId = cart.CartId,
					ProductId = productId,
					Quantity = quantity,
					CreatedAt = DateTime.UtcNow
				};
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
				if (!Request.Headers.XRequestedWith.ToString().Equals("XMLHttpRequest"))
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
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Json(new { success = false, message = "User not logged in" });

			var cartItem = await _context.CartItems
				.Include(ci => ci.Cart)
				.ThenInclude(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == user.Id);

			if (cartItem == null)
				return Json(new { success = false, message = "Cart item not found" });

			cartItem.Quantity++;
			_context.CartItems.Update(cartItem);
			await _context.SaveChangesAsync();

			var newTotal = cartItem.Cart?.CartItems
				.Where(ci => ci.Product != null)
				.Sum(ci => ci.Quantity * ci.Product.Price) ?? 0;

			return Json(new { success = true, quantity = cartItem.Quantity, total = newTotal });
		}

		// Decrement Cart Item
		[HttpPost]
		public async Task<IActionResult> DecrementCartItem(Guid cartItemId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Json(new { success = false, message = "User not logged in" });

			var cartItem = await _context.CartItems
				.Include(ci => ci.Cart)
				.ThenInclude(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == user.Id);

			if (cartItem == null)
				return Json(new { success = false, message = "Cart item not found" });

			cartItem.Quantity = Math.Max(1, cartItem.Quantity - 1); // Prevent quantity going below 1
			_context.CartItems.Update(cartItem);
			await _context.SaveChangesAsync();

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
