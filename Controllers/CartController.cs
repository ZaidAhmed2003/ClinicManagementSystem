using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;


namespace ClinicManagementSystem.Controllers
{
	public class CartController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[HttpPost] 
		public async Task<IActionResult> AddToCart(Guid productId, int quantity)
		{
			// Get the current user
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized("User not found");

			// Fetch the product
			var product = await _context.Products
				.Include(p => p.Inventory) // Assuming Inventory is a navigation property of Product
				.FirstOrDefaultAsync(p => p.ProductId == productId);

			if (product == null) return NotFound("Product not found");

			// Fetch available inventory
			var availableInventory = product.Inventory?.Quantity ?? 0; // Assuming Inventory.Quantity holds the stock

			// Check if the requested quantity exceeds available inventory
			if (quantity <= 0) return BadRequest("Invalid quantity specified.");
			if (availableInventory < quantity) return BadRequest($"Only {availableInventory} items are available for this product.");

			// Check if cart exists for the user
			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.FirstOrDefaultAsync(c => c.UserId == user.Id);

			if (cart == null)
			{
				// Create a new cart if one doesn't exist
				cart = new CartModel
				{
					CartId = Guid.NewGuid(),
					UserId = user.Id,
					Total = 0,
					CreatedAt = DateTime.UtcNow,
					CartItems = new List<CartItemModel>()
				};
				await _context.Carts.AddAsync(cart);
				await _context.SaveChangesAsync(); // Save immediately to track the cart
			}

			// Check if the product is already in the cart
			var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

			int existingQuantityInCart = cartItem?.Quantity ?? 0;
			int totalQuantityAfterAddition = existingQuantityInCart + quantity;

			// Ensure the total quantity does not exceed the available inventory
			if (totalQuantityAfterAddition > availableInventory)
			{
				return BadRequest($"Cannot add {quantity} items. Only {availableInventory - existingQuantityInCart} more items can be added.");
			}

			if (cartItem == null)
			{
				// Add the product as a new cart item
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
				cartItem.Quantity += quantity;
				_context.Entry(cartItem).State = EntityState.Modified;
			}

			// Update the cart's total price
			cart.Total = cart.CartItems.Sum(ci => ci.Quantity * product.Price);
			_context.Entry(cart).State = EntityState.Modified;

			// Save changes
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				return Conflict(new { message = "Failed to update cart. Please try again.", details = ex.Message });
			}

			return RedirectToAction("Shop", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
		{
			var cartItem = await _context.CartItems.FindAsync(cartItemId);
			if (cartItem == null) return NotFound("Cart item not found");

			_context.CartItems.Remove(cartItem);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var cart = await _context.Carts.Include(c => c.CartItems)
										   .ThenInclude(ci => ci.Product)
										   .FirstOrDefaultAsync(c => c.UserId == user.Id);

			return View(cart);
		}

		[HttpPost]
		public async Task<IActionResult> PlaceOrder()
		{
			// Ensure user is authenticated and exists
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized("User not found.");
			}

			// Fetch the user's cart
			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.FirstOrDefaultAsync(c => c.UserId == user.Id);

			// Handle case when cart is empty or doesn't exist
			if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
			{
				return BadRequest("Your cart is empty.");
			}

			// Calculate total amount
			decimal totalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);

			// Create payment details
			var paymentDetail = new PaymentDetailModel
			{
				PaymentId = Guid.NewGuid(),
				UserId = user.Id,
				Amount = totalAmount,
				PaymentMethod = "CreditCard", // Example
				Status = PaymentStatus.Completed,
				PaymentDate = DateTime.UtcNow
			};

			_context.PaymentDetails.Add(paymentDetail);
			await _context.SaveChangesAsync();

			// Create the order
			var order = new OrderModel
			{
				OrderId = Guid.NewGuid(),
				UserId = user.Id,
				PaymentId = paymentDetail.PaymentId,
				TrackingID = Guid.NewGuid().ToString(),
				OrderDate = DateTime.UtcNow,
				TotalAmount = totalAmount,
				Status = OrderStatus.Pending,
				PaymentDetail = paymentDetail // Ensure PaymentDetail is not null
			};

			_context.Orders.Add(order);
			await _context.SaveChangesAsync();

			// Create order items
			foreach (var cartItem in cart.CartItems)
			{
				if (cartItem.Product == null)
				{
					// Handle the case where the product is null
					continue;
				}

				var orderItem = new OrderItemModel
				{
					OrderItemId = Guid.NewGuid(),
					OrderId = order.OrderId,
					ProductId = cartItem.ProductId,
					Price = cartItem.Product.Price,
					Quantity = cartItem.Quantity,
					CreatedAt = DateTime.UtcNow,
					Product = cartItem.Product, // Set Product navigation property
					Order = order // Set Order navigation property
				};

				_context.OrderItems.Add(orderItem);

				// Update product inventory
				var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
				if (product != null)
				{
					product.Inventory.Quantity -= cartItem.Quantity;
					_context.Entry(product).State = EntityState.Modified;
				}
			}

			await _context.SaveChangesAsync();

			// Create transaction
			var transaction = new TransactionModel
			{
				TransactionId = Guid.NewGuid(),
				UserId = user.Id,
				OrderId = order.OrderId,
				Amount = totalAmount,
				TransactionStatus = TransactionStatus.Success,
				TransactionDate = DateTime.UtcNow,
				Order = order
			};

			_context.Transactions.Add(transaction);
			await _context.SaveChangesAsync();

			// Clear cart after order placement
			cart.CartItems.Clear();
			await _context.SaveChangesAsync();

			return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
		}
	}
}