using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicManagementSystem.Controllers
{
	public class ProductController : Controller
	{
		private readonly ApplicationDbContext _context;

		private const int PageSize = 10; // Items per page

		public ProductController(ApplicationDbContext context) => _context = context;

		[HttpGet]
		public async Task<IActionResult> Index(string searchTerm, string categoryFilter, string sortBy, int page = 1)
		{
			// Build base query
			IQueryable<ProductModel> productsQuery = _context.Products
				.Include(p => p.Category)
				.Include(p => p.Inventory)
				.Where(p => p.DeletedAt == null);

			// Apply search filter if provided
			if (!string.IsNullOrEmpty(searchTerm))
				productsQuery = productsQuery.Where(p => p.Name.Contains(searchTerm) || p.SKU.Contains(searchTerm));

			// Apply category filter if provided
			if (!string.IsNullOrEmpty(categoryFilter))
				productsQuery = productsQuery.Where(p => p.Category.Name == categoryFilter);

			// Apply sorting if provided
			productsQuery = sortBy switch
			{
				"price_asc" => productsQuery.OrderBy(p => p.Price),
				"price_desc" => productsQuery.OrderByDescending(p => p.Price),
				"name_asc" => productsQuery.OrderBy(p => p.Name),
				"name_desc" => productsQuery.OrderByDescending(p => p.Name),
				_ => productsQuery.OrderBy(p => p.Name)
			};

			// Get total count for pagination
			var totalItems = await productsQuery.CountAsync();

			// Apply pagination
			var products = await productsQuery
			.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.ToListAsync();

			// ViewBag for filtering options
			ViewBag.Categories = new SelectList(await _context.Product_Category.ToListAsync(), "Name", "Name");
			ViewBag.SearchTerm = searchTerm;
			ViewBag.CategoryFilter = categoryFilter;
			ViewBag.SortBy = sortBy;

			// Pagination setup
			var totalPages = (int)Math.Ceiling((double)totalItems / PageSize);
			ViewBag.TotalPages = totalPages;
			ViewBag.CurrentPage = page;

			return View(products);
		}

		// POST: Index (for form submission with filters or sorting)
		[HttpPost]
		public async Task<IActionResult> IndexPost(string searchTerm, string categoryFilter, string sortBy, int page = 1)
		{
			return await Index(searchTerm, categoryFilter, sortBy, page);
		}

		[HttpGet]
		public async Task<IActionResult> AddProduct()
		{
			ViewBag.Categories = await _context.Product_Category.ToListAsync();
			ViewBag.Discounts = await _context.Product_Discount.ToListAsync();
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddProduct(ProductModel product, IFormFile image)
		{
			ViewBag.Categories = await _context.Product_Category.ToListAsync();
			ViewBag.Discounts = await _context.Product_Discount.ToListAsync();

			var inventory = new ProductInventoryModel
			{
				Quantity = product.Inventory?.Quantity ?? 0,
				CreatedAt = DateTime.UtcNow
			};
			await _context.Product_Inventory.AddAsync(inventory);
			await _context.SaveChangesAsync();

			if (image != null && image.Length > 0)
			{
				string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images/products");
				Directory.CreateDirectory(uploadsFolder);

				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
				string filePath = Path.Combine(uploadsFolder, fileName);

				using var fileStream = new FileStream(filePath, FileMode.Create);
				await image.CopyToAsync(fileStream);

				product.ImagePath = $"/uploads/images/products/{fileName}";
			}

			// Fetch the full Category and Discount objects from the database
			var category = await _context.Product_Category.FindAsync(product.CategoryId);
			var discount = await _context.Product_Discount.FindAsync(product.DiscountId);

			var newProduct = new ProductModel
			{
				Name = product.Name,
				Brand = product.Brand,
				Description = product.Description,
				ShortDescription = product.ShortDescription,
				SKU = await GenerateNextCode(),
				Category = category,
				Price = product.Price,
				Inventory = inventory,
				Discount = discount,
				CreatedAt = DateTime.UtcNow,
				IsAvailable = product.IsAvailable,
				ImagePath = product.ImagePath
			};

			await _context.Products.AddAsync(newProduct);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> EditProduct(Guid id)
		{
			var product = await _context.Products
				.Include(p => p.Category)
				.Include(p => p.Discount)
				.Include(p => p.Inventory)
				.FirstOrDefaultAsync(p => p.ProductId == id);

			if (product == null)
			{
				return NotFound();
			}


			// Populate ViewBag for dropdowns
			ViewBag.Categories = await _context.Product_Category.ToListAsync();
			ViewBag.Discounts = await _context.Product_Discount.ToListAsync();

			return View(product);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditProduct(Guid id, ProductModel product, IFormFile image)
		{
			if (id != product.ProductId)
			{
				return BadRequest();
			}

			var existingProduct = await _context.Products
				.Include(p => p.Category)
				.Include(p => p.Discount)
				.Include(p => p.Inventory)
				.FirstOrDefaultAsync(p => p.ProductId == id);

			if (existingProduct == null)
			{
				return NotFound();
			}

			// Update product properties
			existingProduct.Name = product.Name;
			existingProduct.Brand = product.Brand;
			existingProduct.Description = product.Description;
			existingProduct.ShortDescription = product.ShortDescription;
			existingProduct.Price = product.Price;
			existingProduct.IsAvailable = product.IsAvailable;
			existingProduct.CategoryId = product.CategoryId;
			existingProduct.DiscountId = product.DiscountId;
			existingProduct.ModifiedAt = DateTime.Now;

			// Update Inventory
			if (product.Inventory != null)
			{
				existingProduct.Inventory.Quantity = product.Inventory.Quantity;
				existingProduct.Inventory.ModifiedAt = DateTime.Now;
			}

			// Handle image upload if a new image is provided
			if (image != null && image.Length > 0)
			{
				string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images/products");
				Directory.CreateDirectory(uploadsFolder);

				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
				string filePath = Path.Combine(uploadsFolder, fileName);

				using var fileStream = new FileStream(filePath, FileMode.Create);
				await image.CopyToAsync(fileStream);

				// Delete old image if exists
				if (!string.IsNullOrEmpty(existingProduct.ImagePath))
				{
					string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.ImagePath.TrimStart('/'));
					if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
				}

				existingProduct.ImagePath = $"/uploads/images/products/{fileName}";
			}

			// Fetch the full Category and Discount objects from the database
			var category = await _context.Product_Category.FindAsync(product.CategoryId);
			var discount = await _context.Product_Discount.FindAsync(product.DiscountId);

			existingProduct.Category = category;
			existingProduct.Discount = discount;

			// Save changes
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> DeleteProduct(Guid id)
		{
			var product = await _context.Products
				.Include(p => p.Inventory) // Include Inventory
				.FirstOrDefaultAsync(p => p.ProductId == id);

			if (product == null) return RedirectToAction("Index");
			
			product.DeletedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> DeleteMultiple(List<Guid> SelectedProducts)
		{
			if (SelectedProducts != null && SelectedProducts.Any())
			{
				var productsToDelete = await _context.Products
					.Where(p => SelectedProducts.Contains(p.ProductId))
					.Include(p => p.Inventory) // Include Inventory if necessary
					.ToListAsync();

				foreach (var product in productsToDelete)
				{
					// Mark each product as deleted by setting DeletedAt timestamp
					product.DeletedAt = DateTime.UtcNow;
				}

				await _context.SaveChangesAsync();
			}

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> ViewProductDetails(Guid Id)
		{
			var product = await _context.Products
				.Include(p => p.Category)
				.Include(p => p.Discount)
				.Include(p => p.Inventory)
				.FirstOrDefaultAsync(p => p.ProductId == Id);

			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		private async Task<string> GenerateNextCode()
		{
			var lastProduct = await _context.Products
				.OrderByDescending(m => m.CreatedAt)
				.FirstOrDefaultAsync();

			var prefix = "CMSP-";
			if (lastProduct?.SKU?.StartsWith(prefix) == true)
			{
				var numericPart = lastProduct.SKU.Substring(prefix.Length);
				if (int.TryParse(numericPart, out int number))
					return $"{prefix}{(number + 1):D6}";
			}

			return $"{prefix}00001";
		}
	}
}