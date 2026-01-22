using Microsoft.AspNetCore.Mvc;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.Models;
using StoreManagement.ViewModels;

namespace StoreManagement.Controllers
{
    public class BrandsController(IUnitOfWork unitOfWork, ILogger<BrandsController> logger) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<BrandsController> _logger = logger;
        // GET: BrandsController
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var brands = await _unitOfWork.Brands.FetchAllAsync(c => c.Products!);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                brands = [.. brands.Where(c =>
                    c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )];
            }

            // تحويل إلى ViewModel مع إحصاءات
            var brandVMs = brands.Select(b => new BrandVM
            {
                Id = b.Id,
                Name = b.Name,
                ProductCount = b.Products?.Count ?? 0,
            }).OrderBy(b => b.Id).ToList();

            return View(brandVMs);
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _unitOfWork.Brands.FindAsync(id.Value);

            if (brand == null)
            {
                return NotFound();
            }

            // جلب المنتجات التابعة لهذا القسم

            var products = await _unitOfWork.Products.FetchAllAsync();
            var brandProducts = products.Where(p => p.BrandId == id).ToList();

            ViewBag.brandProducts = brandProducts;

            return View(brand);
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandVM brandVM)
        {
            if (ModelState.IsValid)
            {
                var brand = new Brand
                {
                    Name = brandVM.Name ?? "",
                };

                await _unitOfWork.Brands.AddAsync(brand);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Brand created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(brandVM);
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _unitOfWork.Brands.FindAsync(id.Value);

            if (brand == null)
            {
                return NotFound();
            }

            var brandVM = new BrandVM
            {
                Id = brand.Id,
                Name = brand.Name,
            };

            return View(brandVM);
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandVM brandVM)
        {
            if (id != brandVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var brand = await _unitOfWork.Brands.FindAsync(id);

                    if (brand == null)
                    {
                        return NotFound();
                    }

                    brand.Name = brandVM.Name ?? "";
                    await _unitOfWork.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Brand updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Brand");
                    ModelState.AddModelError("", "An error occurred while updating the brand.");
                }
            }

            return View(brandVM);
        }

        // GET: Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _unitOfWork.Brands.FindAsync(id.Value);

            if (brand == null)
            {
                return NotFound();
            }

            // التحقق إذا كان القسم يحتوي على منتجات
            var products = await _unitOfWork.Products.FetchAllAsync();
            var productCount = products.Count(p => p.BrandId == id);

            ViewBag.ProductCount = productCount;

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _unitOfWork.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            // التحقق إذا كان القسم يحتوي على منتجات قبل الحذف
            var products = await _unitOfWork.Products.FetchAllAsync();
            var hasProducts = products.Any(p => p.BrandId == id);

            if (hasProducts)
            {
                TempData["ErrorMessage"] = "Cannot delete brand that contains products. Please reassign or delete the products first.";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Brands.Remove(brand);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "brand deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
