﻿using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	public class DiscountController : Controller
	{

		private readonly ApplicationDbContext _context;

		public DiscountController(ApplicationDbContext context) => _context = context;


		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var discount = await _context.Product_Discount.ToListAsync();
			return View(discount);
		}

		public IActionResult AddDiscount()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddDiscount(ProductDiscountModel category)
		{
			var newdiscount = new ProductDiscountModel()
			{
				Name = category.Name,
				DiscountValue = category.DiscountValue,	
				 IsActive = category.IsActive = true,
				CreatedAt = DateTime.Now,
			};

			await _context.Product_Discount.AddAsync(newdiscount);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}
	}
}
