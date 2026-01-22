using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreManagement.Areas.Inventory.ViewModels;
using StoreManagement.Areas.Sales.Controllers;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.Models.Inventory;

namespace StoreManagement.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class SuppliersController(IUnitOfWork unitOfWork, ILogger<CustomersController> logger) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CustomersController> _logger = logger;

        // GET: Inventory/Suppliers
        public async Task<IActionResult> Index(string searchTerm, string filter = "all")
        {
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentFilterType"] = filter;

            var suppliers = await _unitOfWork.Suppliers.FetchAllAsync();

            // Apply filters
            if (filter == "active")
                suppliers = [.. suppliers.Where(s => s.IsActive)];
            else if (filter == "inactive")
                suppliers = [.. suppliers.Where(s => !s.IsActive)];

            // Apply search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                suppliers = [.. suppliers.Where(s =>
                    s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (s.ContactPerson??"").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (s.Phone??"").Contains(searchTerm) ||
                    (s.Email??"").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )];
            }

            // تحويل إلى ViewModel مع إحصاءات
            var supplierVMs = suppliers.Select(s => new SupplierVM
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Phone = s.Phone,
                Email = s.Email,
                Website = s.Website,
                Address = s.Address,
                City = s.City,
                Country = s.Country ?? "",
                PostalCode = s.PostalCode,
                TaxNumber = s.TaxNumber,
                PaymentTerms = s.PaymentTerms,
                CreditLimit = s.CreditLimit,
                Balance = s.Balance,
                IsActive = s.IsActive,
                Notes = s.Notes,
                ProductCount = s.Products?.Count ?? 0,
                TransactionCount = s.StockTransactions?.Count ?? 0,
                TotalPurchases = s.StockTransactions?.Sum(st => st.TotalCost) ?? 0
            }).OrderBy(s => s.Name).ToList();

            return View(supplierVMs);
        }

        // GET: Inventory/Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _unitOfWork.Suppliers.FindAsync(id.Value);

            if (supplier == null)
            {
                return NotFound();
            }

            // جلب المنتجات والعمليات
            var products = await _unitOfWork.Products.FetchAllAsync();
            var supplierProducts = products.Where(p => p.SupplierId == id).ToList();

            var transactions = await _unitOfWork.StockTransactions.FetchAllAsync();
            var recentTransactions = transactions
                .Where(st => st.SupplierId == id)
                .OrderByDescending(st => st.TransactionDate)
                .Take(10)
                .ToList();

            ViewBag.SupplierProducts = supplierProducts;
            ViewBag.RecentTransactions = recentTransactions;

            return View(supplier);
        }

        // GET: Inventory/Suppliers/Create
        public IActionResult Create()
        {
            var countries = GetCountries();
            ViewBag.Countries = new SelectList(countries);

            var paymentTerms = new List<string>
            {
                "Net 30",
                "Net 60",
                "Net 90",
                "Cash on Delivery",
                "Advance Payment",
                "50% Advance, 50% on Delivery"
            };
            ViewBag.PaymentTerms = new SelectList(paymentTerms);

            return View();
        }

        // POST: Inventory/Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierVM supplierVM)
        {
            if (ModelState.IsValid)
            {
                var supplier = new Supplier
                {
                    Name = supplierVM.Name ?? "",
                    ContactPerson = supplierVM.ContactPerson,
                    Phone = supplierVM.Phone,
                    Email = supplierVM.Email,
                    Website = supplierVM.Website,
                    Address = supplierVM.Address,
                    City = supplierVM.City,
                    Country = supplierVM.Country,
                    PostalCode = supplierVM.PostalCode,
                    TaxNumber = supplierVM.TaxNumber,
                    PaymentTerms = supplierVM.PaymentTerms,
                    CreditLimit = supplierVM.CreditLimit,
                    Balance = supplierVM.Balance,
                    IsActive = supplierVM.IsActive,
                    Notes = supplierVM.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Suppliers.AddAsync(supplier);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Supplier created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // إعادة تعيين البيانات إذا فشل الـ Validation
            ViewBag.Countries = new SelectList(GetCountries());
            ViewBag.PaymentTerms = new SelectList(GetPaymentTerms());

            return View(supplierVM);
        }

        // GET: Inventory/Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _unitOfWork.Suppliers.FindAsync(id.Value);

            if (supplier == null)
            {
                return NotFound();
            }

            var supplierVM = new SupplierVM
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Website = supplier.Website,
                Address = supplier.Address,
                City = supplier.City,
                Country = supplier.Country ?? "",
                PostalCode = supplier.PostalCode,
                TaxNumber = supplier.TaxNumber,
                PaymentTerms = supplier.PaymentTerms,
                CreditLimit = supplier.CreditLimit,
                Balance = supplier.Balance,
                IsActive = supplier.IsActive,
                Notes = supplier.Notes
            };

            ViewBag.Countries = new SelectList(GetCountries());
            ViewBag.PaymentTerms = new SelectList(GetPaymentTerms());

            return View(supplierVM);
        }

        // POST: Inventory/Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierVM supplierVM)
        {
            if (id != supplierVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var supplier = await _unitOfWork.Suppliers.FindAsync(id);

                    if (supplier == null)
                    {
                        return NotFound();
                    }

                    supplier.Name = supplierVM.Name ?? "";
                    supplier.ContactPerson = supplierVM.ContactPerson;
                    supplier.Phone = supplierVM.Phone;
                    supplier.Email = supplierVM.Email;
                    supplier.Website = supplierVM.Website;
                    supplier.Address = supplierVM.Address;
                    supplier.City = supplierVM.City;
                    supplier.Country = supplierVM.Country;
                    supplier.PostalCode = supplierVM.PostalCode;
                    supplier.TaxNumber = supplierVM.TaxNumber;
                    supplier.PaymentTerms = supplierVM.PaymentTerms;
                    supplier.CreditLimit = supplierVM.CreditLimit;
                    supplier.Balance = supplierVM.Balance;
                    supplier.IsActive = supplierVM.IsActive;
                    supplier.Notes = supplierVM.Notes;
                    supplier.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Supplier updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating supplier");
                    ModelState.AddModelError("", "An error occurred while updating the supplier.");
                }
            }

            ViewBag.Countries = new SelectList(GetCountries());
            ViewBag.PaymentTerms = new SelectList(GetPaymentTerms());

            return View(supplierVM);
        }

        // GET: Inventory/Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _unitOfWork.Suppliers.FindAsync(id.Value);

            if (supplier == null)
            {
                return NotFound();
            }

            // التحقق من الارتباطات
            var products = await _unitOfWork.Products.FetchAllAsync();
            var productCount = products.Count(p => p.SupplierId == id);

            var transactions = await _unitOfWork.StockTransactions.FetchAllAsync();
            var transactionCount = transactions.Count(st => st.SupplierId == id);

            ViewBag.ProductCount = productCount;
            ViewBag.TransactionCount = transactionCount;

            return View(supplier);
        }

        // POST: Inventory/Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _unitOfWork.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            // التحقق من الارتباطات قبل الحذف
            var products = await _unitOfWork.Products.FetchAllAsync();
            var hasProducts = products.Any(p => p.SupplierId == id);

            if (hasProducts)
            {
                TempData["ErrorMessage"] = "Cannot delete supplier that has associated products. Please reassign or delete the products first.";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Suppliers.Remove(supplier);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Supplier deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Inventory/Suppliers/QuickPurchase/5
        public async Task<IActionResult> QuickPurchase(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _unitOfWork.Suppliers.FindAsync(id.Value);

            if (supplier == null)
            {
                return NotFound();
            }

            var products = await _unitOfWork.Products.FetchAllAsync();
            var availableProducts = products.Where(p => p.IsActive).ToList();

            var transactionVM = new StockTransactionVM
            {
                SupplierId = id.Value,
                SupplierName = supplier.Name,
                TransactionType = "Purchase",
                TransactionDate = DateTime.Now,
                Products = availableProducts,
                Suppliers = new List<Supplier> { supplier }
            };

            return View("QuickPurchase", transactionVM);
        }

        // Helper Methods
        private List<string> GetCountries()
        {
            return new List<string>
            {
                "Egypt", "Saudi Arabia", "United Arab Emirates", "Qatar", "Kuwait",
                "Oman", "Jordan", "Lebanon", "Morocco", "Tunisia", "Algeria",
                "Sudan", "Iraq", "Syria", "Yemen", "Libya"
            };
        }

        private List<string> GetPaymentTerms()
        {
            return new List<string>
            {
                "Net 30",
                "Net 60",
                "Net 90",
                "Cash on Delivery",
                "Advance Payment",
                "50% Advance, 50% on Delivery",
                "Custom"
            };
        }

    }
}
