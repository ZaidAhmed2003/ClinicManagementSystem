using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	[Authorize(Roles = "Admin")]
	public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var category = await _context.Product_Category.ToListAsync();
            return View(category);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(ProductCategoryModel category) 
        {
            var newcategory = new ProductCategoryModel() {
                Name = category.Name,
                Description = category.Description,
                CreatedAt = DateTime.Now,
            };

            await _context.Product_Category.AddAsync(newcategory);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(Guid Id)
        {
            var category = await _context.Product_Category.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(ProductCategoryModel category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = await _context.Product_Category.FindAsync(category.CategoryId);
                if (existingCategory == null)
                {
                    return NotFound();
                }

                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.ModifiedAt = DateTime.Now;

                _context.Product_Category.Update(existingCategory);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(Guid Id)
        {
            var category = await _context.Product_Category.FirstOrDefaultAsync(p => p.CategoryId == Id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Product_Category.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
