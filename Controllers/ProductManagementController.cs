using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ClinicManagementSystem.ViewModels.ProductsManagement;

namespace ClinicManagementSystem.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ProductManagementController : Controller
	{
		private readonly ApplicationDbContext _context;
		private const int PageSize = 10;

		public ProductManagementController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> Index(string searchTerm, string categoryFilter, string sortBy, int page = 1)
		{
			var productsQuery = _context.Products
				.Include(p => p.Category)
				.Include(p => p.Inventory)
				.Where(p => p.DeletedAt == null);

			// Filtering
			if (!string.IsNullOrEmpty(searchTerm))
				productsQuery = productsQuery.Where(p => p.Name.Contains(searchTerm) || p.SKU.Contains(searchTerm));

			if (!string.IsNullOrEmpty(categoryFilter))
				productsQuery = productsQuery.Where(p => p.Category.Name == categoryFilter);

			// Sorting
			productsQuery = sortBy switch
			{
				"price_asc" => productsQuery.OrderBy(p => p.Price),
				"price_desc" => productsQuery.OrderByDescending(p => p.Price),
				"name_asc" => productsQuery.OrderBy(p => p.Name),
				"name_desc" => productsQuery.OrderByDescending(p => p.Name),
				_ => productsQuery.OrderBy(p => p.CreatedAt)
			};

			// Pagination
			var totalItems = await productsQuery.CountAsync();
			var products = await productsQuery
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.ToListAsync();

			var categories = await _context.Product_Category
				.Select(c => new SelectListItem { Text = c.Name, Value = c.Name })
				.ToListAsync();

			var viewModel = new ProductIndexViewModel
			{
				Products = products,
				CurrentPage = page,
				TotalPages = (int)Math.Ceiling((double)totalItems / PageSize),
				SearchTerm = searchTerm,
				CategoryFilter = categoryFilter,
				SortBy = sortBy,
				Categories = categories
			};

			return View(viewModel);
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
			if (SelectedProducts != null && SelectedProducts.Count != 0)
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

		[HttpGet]
		public async Task<IActionResult> DeletedProducts(int page = 1)
		{
			var deletedProductsQuery = _context.Products
				.Include(p => p.Category)
				.Where(p => p.DeletedAt != null);

			var totalItems = await deletedProductsQuery.CountAsync();
			var products = await deletedProductsQuery
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.ToListAsync();

			var viewModel = new ProductIndexViewModel
			{
				Products = products,
				CurrentPage = page,
				TotalPages = (int)Math.Ceiling((double)totalItems / PageSize),
			};

			return View(viewModel);
		}

		[HttpGet]
		public async Task<IActionResult> ExportToExcel()
		{
			var products = await _context.Products
				.Include(p => p.Category)
				.Where(p => p.DeletedAt == null)
				.ToListAsync();

			using var package = new OfficeOpenXml.ExcelPackage();
			var worksheet = package.Workbook.Worksheets.Add("Products");

			// Add headers
			worksheet.Cells[1, 1].Value = "Name";
			worksheet.Cells[1, 2].Value = "SKU";
			worksheet.Cells[1, 3].Value = "Category";
			worksheet.Cells[1, 4].Value = "Price";
			worksheet.Cells[1, 5].Value = "Availability";
			worksheet.Cells[1, 6].Value = "Created At";

			// Add data
			for (int i = 0; i < products.Count; i++)
			{
				worksheet.Cells[i + 2, 1].Value = products[i].Name;
				worksheet.Cells[i + 2, 2].Value = products[i].SKU;
				worksheet.Cells[i + 2, 3].Value = products[i].Category.Name;
				worksheet.Cells[i + 2, 4].Value = products[i].Price;
				worksheet.Cells[i + 2, 5].Value = products[i].IsAvailable ? "Active" : "Inactive";
				worksheet.Cells[i + 2, 6].Value = products[i].CreatedAt.ToShortDateString();
			}

			var stream = new MemoryStream();
			package.SaveAs(stream);
			stream.Position = 0;

			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
		}

		[HttpGet]
		public async Task<IActionResult> ExportToPdf()
		{
			var products = await _context.Products
				.Include(p => p.Category)
				.Where(p => p.DeletedAt == null)
				.ToListAsync();

			var stream = new MemoryStream();
			var writer = new iText.Kernel.Pdf.PdfWriter(stream);
			var pdf = new iText.Layout.Document(new iText.Kernel.Pdf.PdfDocument(writer));

			// Add Title
			pdf.Add(new iText.Layout.Element.Paragraph("Products").SetFontSize(20).SetBold());

			// Create Table
			var table = new iText.Layout.Element.Table(6);
			table.AddHeaderCell("Name");
			table.AddHeaderCell("SKU");
			table.AddHeaderCell("Category");
			table.AddHeaderCell("Price");
			table.AddHeaderCell("Availability");
			table.AddHeaderCell("Created At");

			foreach (var product in products)
			{
				table.AddCell(product.Name);
				table.AddCell(product.SKU);
				table.AddCell(product.Category.Name);
				table.AddCell(product.Price.ToString());
				table.AddCell(product.IsAvailable ? "Active" : "Inactive");
				table.AddCell(product.CreatedAt.ToShortDateString());
			}

			pdf.Add(table);
			pdf.Close();

			stream.Position = 0;
			return File(stream, "application/pdf", "Products.pdf");
		}


		private async Task<string> GenerateNextCode()
		{
			var lastProduct = await _context.Products
				.OrderByDescending(m => m.CreatedAt)
				.FirstOrDefaultAsync();

			var prefix = "CMSP-";
			if (lastProduct?.SKU?.StartsWith(prefix) == true)
			{
				var numericPart = lastProduct.SKU[prefix.Length..];
				if (int.TryParse(numericPart, out int number))
					return $"{prefix}{(number + 1):D5}";
			}

			return $"{prefix}00001";
		}
	}
}