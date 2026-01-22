using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreManagement.Areas.Inventory.ViewModels;
using StoreManagement.Areas.Sales.Controllers;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.Models.Inventory;

namespace StoreManagement.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class StockTransactionsController(IUnitOfWork unitOfWork, ILogger<CustomersController> logger) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CustomersController> _logger = logger;

        // GET: Inventory/StockTransactions
        public async Task<IActionResult> Index(StockFilterVM filter)
        {
            var transactions = await _unitOfWork.StockTransactions.FetchAllAsync();
            var products = await _unitOfWork.Products.FetchAllAsync();
            var suppliers = await _unitOfWork.Suppliers.FetchAllAsync();

            // تطبيق الفلاتر
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                transactions = [.. transactions.Where(t =>
                    (t.ReferenceNumber??"").Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (t.Notes??"").Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (t?.Product?.Name??"").Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)
                )];
            }

            if (!string.IsNullOrEmpty(filter.TransactionType) && filter.TransactionType != "all")
            {
                transactions = [.. transactions.Where(t => t.TransactionType == filter.TransactionType)];
            }

            if (filter.StartDate.HasValue)
            {
                transactions = [.. transactions.Where(t => t.TransactionDate >= filter.StartDate.Value)];
            }

            if (filter.EndDate.HasValue)
            {
                transactions = [.. transactions.Where(t => t.TransactionDate <= filter.EndDate.Value)];
            }

            if (filter.ProductId.HasValue)
            {
                transactions = [.. transactions.Where(t => t.ProductId == filter.ProductId.Value)];
            }

            if (filter.SupplierId.HasValue)
            {
                transactions = [.. transactions.Where(t => t.SupplierId == filter.SupplierId.Value)];
            }

            // تحضير البيانات للـ View
            ViewBag.Products = new SelectList(products.Where(p => p.IsActive), "Id", "Name");
            ViewBag.Suppliers = new SelectList(suppliers.Where(s => s.IsActive), "Id", "Name");

            ViewBag.TransactionTypes = new SelectList(new[]
            {
                new { Value = "all", Text = "All Types" },
                new { Value = "Purchase", Text = "Purchase" },
                new { Value = "Sale", Text = "Sale" },
                new { Value = "Return", Text = "Return" },
                new { Value = "Adjustment", Text = "Adjustment" },
                new { Value = "Transfer", Text = "Transfer" }
            }, "Value", "Text", filter.TransactionType ?? "all");

            ViewBag.Filter = filter;

            return View(transactions.OrderByDescending(t => t.TransactionDate).ToList());
        }

        // GET: Inventory/StockTransactions/Create
        public async Task<IActionResult> Create(string transactionType = "Purchase", int? productId = null, int? supplierId = null)
        {
            var products = await _unitOfWork.Products.FetchAllAsync();
            var suppliers = await _unitOfWork.Suppliers.FetchAllAsync();

            var transactionVM = new StockTransactionVM
            {
                TransactionType = transactionType,
                TransactionDate = DateTime.Now,
                Products = products.Where(p => p.IsActive).ToList(),
                Suppliers = suppliers.Where(s => s.IsActive).ToList()
            };

            if (productId.HasValue)
            {
                transactionVM.ProductId = productId.Value;
                var product = products.FirstOrDefault(p => p.Id == productId.Value);
                if (product != null)
                {
                    transactionVM.ProductName = product.Name;
                    transactionVM.CurrentStock = product.StockQuantity;
                    transactionVM.UnitCost = 0;
                }
            }

            if (supplierId.HasValue)
            {
                transactionVM.SupplierId = supplierId.Value;
            }

            return View(transactionVM);
        }

        // POST: Inventory/StockTransactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockTransactionVM transactionVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // إنشاء الـ Transaction
                    var transaction = new StockTransaction
                    {
                        ProductId = transactionVM.ProductId,
                        SupplierId = transactionVM.SupplierId,
                        TransactionType = transactionVM.TransactionType??"",
                        Quantity = transactionVM.Quantity,
                        UnitCost = transactionVM.UnitCost,
                        TransactionDate = transactionVM.TransactionDate,
                        ReferenceNumber = transactionVM.ReferenceNumber,
                        ReferenceType = transactionVM.ReferenceType,
                        ReferenceId = transactionVM.ReferenceId,
                        Notes = transactionVM.Notes,
                        CreatedBy = GetCurrentUserId()
                    };

                    await _unitOfWork.StockTransactions.AddAsync(transaction);

                    // تحديث مخزون المنتج
                    var product = await _unitOfWork.Products.FindAsync(transactionVM.ProductId);
                    if (product != null)
                    {
                        switch (transactionVM.TransactionType)
                        {
                            case "Purchase":
                            case "Return":
                                product.StockQuantity += transactionVM.Quantity;
                                break;
                            case "Sale":
                                product.StockQuantity -= transactionVM.Quantity;
                                break;
                            case "Adjustment":
                                // في حالة الضبط، الكمية تحدد إذا كانت زيادة أو نقصان
                                product.StockQuantity = transactionVM.Quantity;
                                break;
                        }

                    }

                    await _unitOfWork.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Stock transaction created successfully!";

                    // التوجيه بناءً على نوع العملية
                    if (transactionVM.TransactionType == "Purchase" && transactionVM.SupplierId.HasValue)
                    {
                        return RedirectToAction("Details", "Suppliers", new { id = transactionVM.SupplierId.Value });
                    }
                    else if (transactionVM.ProductId > 0)
                    {
                        return RedirectToAction("Details", "Products", new { id = transactionVM.ProductId });
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating stock transaction");
                    ModelState.AddModelError("", "An error occurred while creating the stock transaction.");
                }
            }

            // إذا فشل الـ Validation، إعادة تحميل البيانات
            var products = await _unitOfWork.Products.FetchAllAsync();
            var suppliers = await _unitOfWork.Suppliers.FetchAllAsync();

            transactionVM.Products = products.Where(p => p.IsActive).ToList();
            transactionVM.Suppliers = suppliers.Where(s => s.IsActive).ToList();

            return View(transactionVM);
        }

        // GET: Inventory/StockTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _unitOfWork.StockTransactions.FindAsync(id.Value);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Inventory/StockTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _unitOfWork.StockTransactions.FindAsync(id.Value);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Inventory/StockTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _unitOfWork.StockTransactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            // إرجاع المخزون قبل الحذف
            var product = await _unitOfWork.Products.FindAsync(transaction.ProductId);
            if (product != null)
            {
                switch (transaction.TransactionType)
                {
                    case "Purchase":
                    case "Return":
                        product.StockQuantity -= transaction.Quantity;
                        break;
                    case "Sale":
                        product.StockQuantity += transaction.Quantity;
                        break;
                }
            }

            _unitOfWork.StockTransactions.Remove(transaction);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Stock transaction deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // AJAX: جلب تفاصيل المنتج
        [HttpGet]
        public async Task<IActionResult> GetProductDetails(int productId)
        {
            var product = await _unitOfWork.Products.FindAsync(productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            return Json(new
            {
                success = true,
                name = product.Name,
                currentStock = product.StockQuantity,
                sellingPrice = product.Price
            });
        }

        // GET: Inventory/StockTransactions/BulkPurchase
        public async Task<IActionResult> BulkPurchase()
        {
            var products = await _unitOfWork.Products.FetchAllAsync();
            var suppliers = await _unitOfWork.Suppliers.FetchAllAsync();

            ViewBag.Products = products.Where(p => p.IsActive).ToList();
            ViewBag.Suppliers = new SelectList(suppliers.Where(s => s.IsActive), "Id", "Name");

            return View();
        }

        // POST: Inventory/StockTransactions/BulkPurchase
        [HttpPost]
        public async Task<IActionResult> BulkPurchase(int supplierId, List<BulkPurchaseItem> items)
        {
            if (items == null || !items.Any())
            {
                return Json(new { success = false, message = "No items provided" });
            }

            try
            {
                var referenceNumber = $"BULK-{DateTime.Now:yyyyMMddHHmmss}";

                foreach (var item in items)
                {
                    if (item.ProductId > 0 && item.Quantity > 0 && item.UnitCost > 0)
                    {
                        var transaction = new StockTransaction
                        {
                            ProductId = item.ProductId,
                            SupplierId = supplierId,
                            TransactionType = "Purchase",
                            Quantity = item.Quantity,
                            UnitCost = item.UnitCost,
                            TransactionDate = DateTime.Now,
                            ReferenceNumber = referenceNumber,
                            ReferenceType = "BulkPurchase",
                            Notes = item.Notes,
                            CreatedBy = GetCurrentUserId()
                        };

                        await _unitOfWork.StockTransactions.AddAsync(transaction);

                        // تحديث المخزون
                        var product = await _unitOfWork.Products.FindAsync(item.ProductId);
                        product?.StockQuantity += item.Quantity;
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Bulk purchase completed successfully",
                    referenceNumber = referenceNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bulk purchase");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // Helper Classes
        public class BulkPurchaseItem
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitCost { get; set; }
            public string? Notes { get; set; }
        }

        // Helper Method (لاحظ: تحتاج لتنفيذ Authentication)
        private static int? GetCurrentUserId()
        {
            // هذا مثال - تحتاج لتنفيذ النظام الحقيقي
            // return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return 1; // مؤقت
        }

    }
}
