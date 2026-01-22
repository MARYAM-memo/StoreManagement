using Microsoft.AspNetCore.Mvc;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.Models;
using StoreManagement.ViewModels;

namespace StoreManagement.Controllers
{
    public class CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CategoriesController> _logger = logger;
        // GET: CategoriesController
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var categories = await _unitOfWork.Categories.FetchAllAsync(c => c.Products!);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                categories = [.. categories.Where(c =>
                    c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.Description != null && c.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )];
            }

            // تحويل إلى ViewModel مع إحصاءات
            var categoryVMs = categories.Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                ProductCount = c.Products?.Count ?? 0,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).OrderBy(c => c.Id).ToList();

            return View(categoryVMs);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.Categories.FindAsync(id.Value);

            if (category == null)
            {
                return NotFound();
            }

            // جلب المنتجات التابعة لهذا القسم

            var products = await _unitOfWork.Products.FetchAllAsync();
            var categoryProducts = products.Where(p => p.CategoryId == id).ToList();

            ViewBag.CategoryProducts = categoryProducts;

            return View(category);
        }


        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM categoryVM)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = categoryVM.Name ?? "",
                    Description = categoryVM.Description,
                    IsActive = categoryVM.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(categoryVM);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.Categories.FindAsync(id.Value);

            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = new CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };

            return View(categoryVM);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryVM categoryVM)
        {
            if (id != categoryVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var category = await _unitOfWork.Categories.FindAsync(id);

                    if (category == null)
                    {
                        return NotFound();
                    }

                    category.Name = categoryVM.Name ?? "";
                    category.Description = categoryVM.Description;
                    category.IsActive = categoryVM.IsActive;
                    category.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating category");
                    ModelState.AddModelError("", "An error occurred while updating the category.");
                }
            }

            return View(categoryVM);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.Categories.FindAsync(id.Value);

            if (category == null)
            {
                return NotFound();
            }

            // التحقق إذا كان القسم يحتوي على منتجات
            var products = await _unitOfWork.Products.FetchAllAsync();
            var productCount = products.Count(p => p.CategoryId == id);

            ViewBag.ProductCount = productCount;

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _unitOfWork.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            // التحقق إذا كان القسم يحتوي على منتجات قبل الحذف
            var products = await _unitOfWork.Products.FetchAllAsync();
            var hasProducts = products.Any(p => p.CategoryId == id);

            if (hasProducts)
            {
                TempData["ErrorMessage"] = "Cannot delete category that contains products. Please reassign or delete the products first.";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.Categories.FindAsync(id.Value);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            var category = await _unitOfWork.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deactivated successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
