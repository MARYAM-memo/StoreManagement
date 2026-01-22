using Microsoft.AspNetCore.Mvc;
using StoreManagement.Common;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.Models;
using StoreManagement.ViewModels;

namespace StoreManagement.Controllers
{
    public class ProductsController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger, IWebHostEnvironment host) : Controller
    {

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        readonly IWebHostEnvironment _host = host;
        private readonly ILogger<ProductsController> _logger = logger;


        // GET: Products
        public async Task<IActionResult> Index(string searchTerm, string sortOrder)
        {
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "price" ? "price_desc" : "price";
            ViewData["StockSortParm"] = sortOrder == "stock" ? "stock_desc" : "stock";
            ViewData["CurrentFilter"] = searchTerm;

            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = await _unitOfWork.Products.SearchProductsAsync(searchTerm);
            }
            else
            {
                products = await _unitOfWork.Products.GetProductsWithDetailsAsync();
            }

            products = sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "price" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "stock" => products.OrderBy(p => p.StockQuantity),
                "stock_desc" => products.OrderByDescending(p => p.StockQuantity),
                _ => products.OrderBy(p => p.Id)
            };

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.Products.GetProductWithDetailsAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create(int? categoryId, int? brandId)
        {
            var categories = await _unitOfWork.Categories.FetchAllAsync();
            var brands = await _unitOfWork.Brands.FetchAllAsync();

            var viewModel = new ProductVM
            {
                Categories = categories,
                Brands = brands
            };
            if (categoryId.HasValue)
            {
                viewModel.CategoryId = categoryId.Value;
            }

            if (brandId.HasValue)
            {
                viewModel.BrandId = brandId.Value;
            }


            return View(viewModel);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = productVM.Name ?? "",

                    Description = productVM.Description,
                    Price = productVM.Price,

                    StockQuantity = productVM.StockQuantity,

                    CategoryId = productVM.CategoryId,
                    BrandId = productVM.BrandId,
                    IsActive = productVM.IsActive,
                    CreatedAt = DateTime.Now
                };
                product.ImageUrl = imageFile.ExtractImagePath(_host);
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdown data if validation fails
            productVM.Categories = await _unitOfWork.Categories.FetchAllAsync();
            productVM.Brands = await _unitOfWork.Brands.FetchAllAsync();

            return View(productVM);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.Products.FindAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            var categories = await _unitOfWork.Categories.FetchAllAsync();
            var brands = await _unitOfWork.Brands.FetchAllAsync();

            var productVM = new ProductVM
            {
                Id = product.Id,
                Name = product.Name,

                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                Categories = categories,
                Brands = brands
            };

            return View(productVM);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVM productVM, IFormFile? imageFile)
        {
            if (id != productVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _unitOfWork.Products.FindAsync(id);

                    if (product == null)
                    {
                        return NotFound();
                    }

                    product.Name = productVM.Name ?? "";
                    product.Description = productVM.Description;
                    product.Price = productVM.Price;
                    product.StockQuantity = productVM.StockQuantity;
                    product.CategoryId = productVM.CategoryId;
                    product.BrandId = productVM.BrandId;
                    System.Console.WriteLine($"imageFile: {imageFile}");
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        product.ImageUrl = imageFile.ExtractImagePath(_host);
                    }
                    else { product.ImageUrl = productVM.ImageUrl; }
                    System.Console.WriteLine($"product.ImageUrl:{product.ImageUrl}");
                    product.IsActive = productVM.IsActive;
                    product.UpdatedAt = DateTime.Now;
                    await _unitOfWork.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product");
                    ModelState.AddModelError("", "An error occurred while updating the product.");
                }
            }

            // Reload dropdown data if validation fails
            productVM.Categories = await _unitOfWork.Categories.FetchAllAsync();
            productVM.Brands = await _unitOfWork.Brands.FetchAllAsync();

            return View(productVM);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.Products.GetProductWithDetailsAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _unitOfWork.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Soft delete - just deactivate
            product.IsActive = false;
            product.UpdatedAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product deactivated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Restock/5
        public async Task<IActionResult> Restock(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.Products.FindAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.ProductName = product.Name;
            ViewBag.CurrentStock = product.StockQuantity;

            return View();
        }

        // POST: Products/Restock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restock(int id, int quantity)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError("quantity", "Quantity must be greater than 0");
                return View();
            }

            var product = await _unitOfWork.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            product.StockQuantity += quantity;
            product.UpdatedAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{quantity} units added to stock. New stock: {product.StockQuantity}";
            return RedirectToAction(nameof(Index));
        }
    }
}


