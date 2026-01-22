using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreManagement.Areas.Sales.ViewModels;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.Models;

namespace StoreManagement.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class OrdersController(IUnitOfWork unitOfWork, ILogger<OrdersController> logger) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<OrdersController> _logger = logger;

        // GET: Sales/Orders
        public async Task<IActionResult> Index(OrderFilterVM filter)
        {
            var orders = await _unitOfWork.Orders.FetchAllAsync();
            var customers = await _unitOfWork.Customers.FetchAllAsync();

            // تطبيق الفلاتر
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                orders = [.. orders.Where(o =>
                    o.OrderNumber.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (o.Customer?.Name??"").Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)
                )];
            }

            if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "all")
            {
                orders = [.. orders.Where(o => o.Status == filter.Status)];
            }

            if (!string.IsNullOrEmpty(filter.PaymentStatus) && filter.PaymentStatus != "all")
            {
                orders = [.. orders.Where(o => o.PaymentStatus == filter.PaymentStatus)];
            }

            if (filter.StartDate.HasValue)
            {
                orders = [.. orders.Where(o => o.OrderDate >= filter.StartDate.Value)];
            }

            if (filter.EndDate.HasValue)
            {
                orders = [.. orders.Where(o => o.OrderDate <= filter.EndDate.Value)];
            }

            if (filter.CustomerId.HasValue)
            {
                orders = [.. orders.Where(o => o.CustomerId == filter.CustomerId.Value)];
            }

            // تحضير البيانات للـ View
            ViewBag.Customers = new SelectList(customers, "Id", "Name");
            ViewBag.Statuses = new SelectList(new[]
            {
                new { Value = "all", Text = "All Statuses" },
                new { Value = "Pending", Text = "Pending" },
                new { Value = "Processing", Text = "Processing" },
                new { Value = "Completed", Text = "Completed" },
                new { Value = "Cancelled", Text = "Cancelled" },
                new { Value = "Refunded", Text = "Refunded" }
            }, "Value", "Text", filter.Status ?? "all");

            ViewBag.PaymentStatuses = new SelectList(new[]
            {
                new { Value = "all", Text = "All Payment Statuses" },
                new { Value = "Pending", Text = "Pending" },
                new { Value = "Paid", Text = "Paid" },
                new { Value = "Failed", Text = "Failed" },
                new { Value = "Refunded", Text = "Refunded" }
            }, "Value", "Text", filter.PaymentStatus ?? "all");

            ViewBag.Filter = filter;

            return View(orders.OrderByDescending(o => o.OrderDate).ToList());
        }

        // GET: Sales/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _unitOfWork.Orders.FindAsync(id.Value);

            if (order == null)
            {
                return NotFound();
            }

            // جلب الـ Order Items مع تفاصيل المنتجات
            var orderItems = await _unitOfWork.OrderItems.FetchAllAsync();
            var products = await _unitOfWork.Products.FetchAllAsync();

            var items = orderItems
                .Where(oi => oi.OrderId == id)
                .Join(products,
                    oi => oi.ProductId,
                    p => p.Id,
                    (oi, p) => new OrderItemVM
                    {
                        Id = oi.Id,
                        ProductId = p.Id,
                        ProductName = p.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Discount = oi.Discount ?? 0
                    }).ToList();

            ViewBag.OrderItems = items;

            return View(order);
        }

        // GET: Sales/Orders/Create
        public async Task<IActionResult> Create(int? customerId)
        {
            var customers = await _unitOfWork.Customers.FetchAllAsync();
            var products = await _unitOfWork.Products.FetchAllAsync();

            var orderVM = new OrderVM
            {
                Customers = customers.Where(c => c.IsActive).ToList(),
                Products = products.Where(p => p.IsActive).ToList(),
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                PaymentStatus = "Pending"
            };

            if (customerId.HasValue)
            {
                orderVM.CustomerId = customerId.Value;
                var customer = customers.FirstOrDefault(c => c.Id == customerId.Value);
                if (customer != null)
                {
                    orderVM.ShippingAddress = customer.Address;
                    orderVM.BillingAddress = customer.Address;
                }
            }

            return View(orderVM);
        }

        // POST: Sales/Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderVM orderVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // إنشاء الـ Order
                    var order = new Order
                    {
                        CustomerId = orderVM.CustomerId,
                        OrderNumber = GenerateOrderNumber(),
                        OrderDate = orderVM.OrderDate,
                        Status = orderVM.Status,
                        ShippingAddress = orderVM.ShippingAddress,
                        BillingAddress = orderVM.BillingAddress,
                        PaymentMethod = orderVM.PaymentMethod,
                        PaymentStatus = orderVM.PaymentStatus,
                        ShippingCost = orderVM.ShippingCost,
                        TaxAmount = orderVM.TaxAmount,
                        DiscountAmount = orderVM.DiscountAmount,
                        TotalAmount = orderVM.TotalAmount,
                        Notes = orderVM.Notes
                    };

                    await _unitOfWork.Orders.AddAsync(order);
                    await _unitOfWork.SaveChangesAsync();

                    // إضافة الـ Order Items
                    if (orderVM.OrderItems != null && orderVM.OrderItems.Any())
                    {
                        foreach (var item in orderVM.OrderItems)
                        {
                            var orderItem = new OrderItem
                            {
                                OrderId = order.Id,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                Discount = item.Discount > 0 ? item.Discount : (decimal?)null,
                                ProductName = item.ProductName
                            };

                            await _unitOfWork.OrderItems.AddAsync(orderItem);

                            // تحديث المخزون إذا كانت حالة الطلب "Completed"
                            if (order.Status == "Completed")
                            {
                                var product = await _unitOfWork.Products.FindAsync(item.ProductId);
                                product?.StockQuantity -= item.Quantity;
                            }
                        }

                        await _unitOfWork.SaveChangesAsync();
                    }

                    // تحديث إحصائيات العميل
                    var customer = await _unitOfWork.Customers.FindAsync(order.CustomerId);
                    if (customer != null)
                    {
                        customer.TotalOrders++;
                        customer.TotalSpent += order.TotalAmount;
                        customer.LastPurchaseDate = order.OrderDate;

                        await _unitOfWork.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = $"Order #{order.OrderNumber} created successfully!";
                    return RedirectToAction(nameof(Details), new { id = order.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating order");
                    ModelState.AddModelError("", "An error occurred while creating the order.");
                }
            }

            // إذا فشل الـ Validation، إعادة تحميل البيانات
            var customers = await _unitOfWork.Customers.FetchAllAsync();
            var products = await _unitOfWork.Products.FetchAllAsync();

            orderVM.Customers = customers.Where(c => c.IsActive).ToList();
            orderVM.Products = products.Where(p => p.IsActive).ToList();

            return View(orderVM);
        }

        // GET: Sales/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _unitOfWork.Orders.FindAsync(id.Value);

            if (order == null)
            {
                return NotFound();
            }

            // جلب الـ Order Items
            var orderItems = await _unitOfWork.OrderItems.FetchAllAsync();
            var items = orderItems.Where(oi => oi.OrderId == id).ToList();

            var customers = await _unitOfWork.Customers.FetchAllAsync();
            var products = await _unitOfWork.Products.FetchAllAsync();

            var orderVM = new OrderVM
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus ?? "",
                ShippingCost = order.ShippingCost,
                TaxAmount = order.TaxAmount,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                Notes = order.Notes,
                Customers = customers.Where(c => c.IsActive).ToList(),
                Products = products.Where(p => p.IsActive).ToList()
            };

            // تحويل Order Items إلى ViewModel
            foreach (var item in items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                orderVM.OrderItems.Add(new OrderItemVM
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount ?? 0,
                    AvailableStock = product?.StockQuantity ?? 0
                });
            }

            return View(orderVM);
        }

        // POST: Sales/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderVM orderVM)
        {
            if (id != orderVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var order = await _unitOfWork.Orders.FindAsync(id);

                    if (order == null)
                    {
                        return NotFound();
                    }

                    // حفظ الحالة القديمة للتأكد من تحديث المخزون بشكل صحيح
                    var oldStatus = order.Status;

                    order.CustomerId = orderVM.CustomerId;
                    order.Status = orderVM.Status;
                    order.ShippingAddress = orderVM.ShippingAddress;
                    order.BillingAddress = orderVM.BillingAddress;
                    order.PaymentMethod = orderVM.PaymentMethod;
                    order.PaymentStatus = orderVM.PaymentStatus;
                    order.ShippingCost = orderVM.ShippingCost;
                    order.TaxAmount = orderVM.TaxAmount;
                    order.DiscountAmount = orderVM.DiscountAmount;
                    order.TotalAmount = orderVM.TotalAmount;
                    order.Notes = orderVM.Notes;

                    // إذا تغيرت الحالة من/إلى "Completed"، تحديث المخزون
                    if (oldStatus != "Completed" && orderVM.Status == "Completed")
                    {
                        // خصم الكميات من المخزون
                        await UpdateStockForOrder(order.Id, false);
                    }
                    else if (oldStatus == "Completed" && orderVM.Status != "Completed")
                    {
                        // إرجاع الكميات إلى المخزون
                        await UpdateStockForOrder(order.Id, true);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Order #{order.OrderNumber} updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = order.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating order");
                    ModelState.AddModelError("", "An error occurred while updating the order.");
                }
            }

            // إذا فشل الـ Validation، إعادة تحميل البيانات
            var customers = await _unitOfWork.Customers.FetchAllAsync();
            var products = await _unitOfWork.Products.FetchAllAsync();

            orderVM.Customers = customers.Where(c => c.IsActive).ToList();
            orderVM.Products = products.Where(p => p.IsActive).ToList();

            return View(orderVM);
        }

        // GET: Sales/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _unitOfWork.Orders.FindAsync(id.Value);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Sales/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _unitOfWork.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // إرجاع الكميات إلى المخزون إذا كانت حالة الطلب "Completed"
            if (order.Status == "Completed")
            {
                await UpdateStockForOrder(order.Id, true);
            }

            // حذف الـ Order Items أولاً
            var orderItems = await _unitOfWork.OrderItems.FetchAllAsync();
            var items = orderItems.Where(oi => oi.OrderId == id).ToList();

            foreach (var item in items)
            {
                _unitOfWork.OrderItems.Remove(item);
            }

            // تحديث إحصائيات العميل
            var customer = await _unitOfWork.Customers.FindAsync(order.CustomerId);
            if (customer != null)
            {
                customer.TotalOrders--;
                customer.TotalSpent -= order.TotalAmount;
            }

            // حذف الطلب
            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order #{order.OrderNumber} deleted successfully!";
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
                price = product.Price,
                stock = product.StockQuantity
            });
        }

        // AJAX: تحديث حالة الطلب
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            var order = await _unitOfWork.Orders.FindAsync(orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            var oldStatus = order.Status;
            order.Status = status;

            // تحديث المخزون بناءً على تغير الحالة
            if (oldStatus != "Completed" && status == "Completed")
            {
                await UpdateStockForOrder(orderId, false);
            }
            else if (oldStatus == "Completed" && status != "Completed")
            {
                await UpdateStockForOrder(orderId, true);
            }

            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "Status updated successfully" });
        }

        // GET: Sales/Orders/Print/5
        public async Task<IActionResult> Print(int id)
        {
            var order = await _unitOfWork.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // جلب الـ Order Items مع تفاصيل المنتجات
            var orderItems = await _unitOfWork.OrderItems.FetchAllAsync();
            var products = await _unitOfWork.Products.FetchAllAsync();

            var items = orderItems
                .Where(oi => oi.OrderId == id)
                .Join(products,
                    oi => oi.ProductId,
                    p => p.Id,
                    (oi, p) => new OrderItemVM
                    {
                        ProductName = p.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Discount = oi.Discount ?? 0
                    }).ToList();

            ViewBag.OrderItems = items;
            ViewBag.PrintMode = true;

            return View("Details", order);
        }

        // Private Methods
        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        }

        private async Task UpdateStockForOrder(int orderId, bool addToStock)
        {
            var orderItems = await _unitOfWork.OrderItems.FetchAllAsync();
            var items = orderItems.Where(oi => oi.OrderId == orderId).ToList();

            foreach (var item in items)
            {
                var product = await _unitOfWork.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    if (addToStock)
                    {
                        product.StockQuantity += item.Quantity;
                    }
                    else
                    {
                        product.StockQuantity -= item.Quantity;
                    }
                }
            }
        }

    }
}
