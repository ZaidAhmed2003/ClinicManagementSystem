//using ClinicManagementSystem.Data;
//using ClinicManagementSystem.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace ClinicManagementSystem.Controllers
//{
//    public class MedicineController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public MedicineController(ApplicationDbContext context) => _context = context;

//        [HttpGet]
//        public async Task<IActionResult> Index()
//        {

//            var medicines = await _context.Medicines.ToListAsync();
//            return View(medicines);
//        }

//        [HttpGet]
//        public IActionResult AddProduct()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> AddProduct(ClinicManagementSystem.Models.MedicineModel model, IFormFile image)
//        {
//            // Generate the next unique code
//            model.Code = await GenerateNextCode();

//            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images/medicines");

//            if (!Directory.Exists(uploadsFolder))
//            {
//                Directory.CreateDirectory(uploadsFolder);
//            }

//            if (image != null && image.Length > 0)
//            {
//                // Generate a unique file name by adding a GUID to avoid name collisions
//                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

//                // Combine the folder path with the file name
//                string filePath = Path.Combine(uploadsFolder, fileName);

//                // Copy the file to the target location
//                using (var fileStream = new FileStream(filePath, FileMode.Create))
//                {
//                    await image.CopyToAsync(fileStream);
//                }

//                // Save the relative image path in the model
//                model.ImagePath = $"/uploads/images/medicines/{fileName}";
//            }

//            // Create a new medicine entry
//            var medicine = new ClinicManagementSystem.Models.MedicineModel
//            {

//                Code = model.Code,
//                Name = model.Name,
//                Brand = model.Brand,
//                Description = model.Description,
//                Price = model.Price,
//                Quantity = model.Quantity,
//                IsAvailable = model.Quantity > 0,
//                ImagePath = model.ImagePath,
//            };

//            await _context.Medicines.AddAsync(medicine); // Add the new medicine to the database context
//            await _context.SaveChangesAsync(); // Save changes to the database

//            return RedirectToAction("Index");

//        }

//        public IActionResult ViewProductDetail()
//        {
//            return View();
//        }

//        public IActionResult EditProduct()
//        {
//            return View();
//        }


//        [HttpGet]
//        public async Task<IActionResult> DeleteProduct(Guid Id)
//        {
//            // Fetch medicine to delete
//            var medicine = await _context.Medicines.FirstOrDefaultAsync(m => m.MedicineId == Id);

//            if (medicine != null)
//            {
//                // Check if the ImagePath is set and the file exists
//                if (!string.IsNullOrEmpty(medicine.ImagePath))
//                {
//                    // Get the full path of the image file
//                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", medicine.ImagePath.TrimStart('/'));

//                    if (System.IO.File.Exists(fullPath))
//                    {
//                        // Delete the image file from the server
//                        System.IO.File.Delete(fullPath);
//                    }
//                }

//                // Remove the medicine from the database
//                _context.Medicines.Remove(medicine);
//                await _context.SaveChangesAsync();
//            }

//            return RedirectToAction("Index");
//        }


//        private async Task<string> GenerateNextCode()
//        {
//            // Get the latest medicine by creation date to retrieve the last used code
//            var lastMedicine = await _context.Medicines
//                .OrderByDescending(m => m.CreatedAt)
//                .FirstOrDefaultAsync();

//            if (lastMedicine == null || string.IsNullOrEmpty(lastMedicine.Code) || lastMedicine.Code.Length < 5)
//            {
//                // Start from "CMSM-000001" if no valid code exists
//                return "CMSM-000001";
//            }

//            // Ensure the code starts with "CMSM-" and has a numeric part
//            var prefix = "CMSM-";
//            if (lastMedicine.Code.StartsWith(prefix) && lastMedicine.Code.Length >= prefix.Length + 6)
//            {
//                // Extract the numeric part safely
//                var numericPart = lastMedicine.Code.Substring(prefix.Length);
//                if (int.TryParse(numericPart, out int number))
//                {
//                    // Increment the numeric part by 1 and format with leading zeros
//                    var nextNumber = number + 1;
//                    return $"{prefix}{nextNumber:D6}";
//                }
//            }

//            // Fallback to "CMSM-000001" if parsing fails
//            return "CMSM-000001";
//        }

//    }
//}

