using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreManagement.Areas.Sales.ViewModels;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.Models;

namespace StoreManagement.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class CustomersController(IUnitOfWork unitOfWork, ILogger<CustomersController> logger) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CustomersController> _logger = logger;
        // GET: Sales/Customers
        public async Task<IActionResult> Index(string searchTerm, string filter = "all")
        {
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentFilterType"] = filter;

            var customers = await _unitOfWork.Customers.FetchAllAsync();

            // Apply filters
            if (filter == "active")
                customers = [.. customers.Where(c => c.IsActive)];
            else if (filter == "inactive")
                customers = [.. customers.Where(c => !c.IsActive)];

            // Apply search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                customers = [.. customers.Where(c =>
                    c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email??"").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.Phone??"").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )];
            }

            // تحويل إلى ViewModel
            var customerVMs = customers.Select(c => new CustomerVM
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                City = c.City,
                Country = c.Country ?? "",
                PostalCode = c.PostalCode,
                IsActive = c.IsActive,
                Notes = c.Notes,
                TotalOrders = c.TotalOrders,
                TotalSpent = c.TotalSpent,
                LastPurchaseDate = c.LastPurchaseDate
            }).OrderByDescending(c => c.LastPurchaseDate ?? DateTime.MinValue).ToList();

            return View(customerVMs);
        }

        // GET: Sales/Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _unitOfWork.Customers.FindAsync(id.Value);

            if (customer == null)
            {
                return NotFound();
            }

            // جلب طلبات العميل
            var orders = await _unitOfWork.Orders.FetchAllAsync();
            var customerOrders = orders.Where(o => o.CustomerId == id)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToList();

            ViewBag.CustomerOrders = customerOrders;

            return View(customer);
        }

        // GET: Sales/Customers/Create
        public IActionResult Create()
        {
            // قائمة الدول (يمكن جلبها من قاعدة بيانات)
            var countries = new List<string>
            {
                "Egypt", "Saudi Arabia", "UAE", "Qatar", "Kuwait",
                "Oman", "Jordan", "Lebanon", "Morocco", "Tunisia"
            };

            ViewBag.Countries = new SelectList(countries);

            return View();
        }

        // POST: Sales/Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerVM customerVM)
        {
            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    Name = customerVM.Name,
                    Phone = customerVM.Phone,
                    Email = customerVM.Email,
                    Address = customerVM.Address,
                    City = customerVM.City,
                    Country = customerVM.Country,
                    PostalCode = customerVM.PostalCode,
                    IsActive = customerVM.IsActive,
                    Notes = customerVM.Notes,
                    CreatedAt = DateTime.UtcNow,
                    TotalOrders = 0,
                    TotalSpent = 0
                };

                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Customer created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // إعادة تعيين قائمة الدول إذا فشل الـ Validation
            var countries = new List<string>
            {
                "Egypt", "Saudi Arabia", "UAE", "Qatar", "Kuwait",
                "Oman", "Jordan", "Lebanon", "Morocco", "Tunisia"
            };

            ViewBag.Countries = new SelectList(countries);

            return View(customerVM);
        }

        // GET: Sales/Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _unitOfWork.Customers.FindAsync(id.Value);

            if (customer == null)
            {
                return NotFound();
            }

            var customerVM = new CustomerVM
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                City = customer.City,
                Country = customer.Country??"",
                PostalCode = customer.PostalCode,
                IsActive = customer.IsActive,
                Notes = customer.Notes
            };

            // قائمة الدول
            var countries = new List<string>
            {
                "Egypt", "Saudi Arabia", "UAE", "Qatar", "Kuwait",
                "Oman", "Jordan", "Lebanon", "Morocco", "Tunisia"
            };

            ViewBag.Countries = new SelectList(countries);

            return View(customerVM);
        }

        // POST: Sales/Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerVM customerVM)
        {
            if (id != customerVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = await _unitOfWork.Customers.FindAsync(id);

                    if (customer == null)
                    {
                        return NotFound();
                    }

                    customer.Name = customerVM.Name;
                    customer.Phone = customerVM.Phone;
                    customer.Email = customerVM.Email;
                    customer.Address = customerVM.Address;
                    customer.City = customerVM.City;
                    customer.Country = customerVM.Country;
                    customer.PostalCode = customerVM.PostalCode;
                    customer.IsActive = customerVM.IsActive;
                    customer.Notes = customerVM.Notes;

                    await _unitOfWork.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Customer updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating customer");
                    ModelState.AddModelError("", "An error occurred while updating the customer.");
                }
            }

            // إعادة تعيين قائمة الدول إذا فشل الـ Validation
            var countries = new List<string>
            {
                "Egypt", "Saudi Arabia", "UAE", "Qatar", "Kuwait",
                "Oman", "Jordan", "Lebanon", "Morocco", "Tunisia"
            };

            ViewBag.Countries = new SelectList(countries);

            return View(customerVM);
        }

        // GET: Sales/Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _unitOfWork.Customers.FindAsync(id.Value);

            if (customer == null)
            {
                return NotFound();
            }

            // التحقق إذا كان العميل لديه طلبات
            var orders = await _unitOfWork.Orders.FetchAllAsync();
            var orderCount = orders.Count(o => o.CustomerId == id);

            ViewBag.OrderCount = orderCount;
            ViewBag.TotalSpent = customer.TotalSpent;

            return View(customer);
        }

        // POST: Sales/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _unitOfWork.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            // التحقق إذا كان العميل لديه طلبات قبل الحذف
            var orders = await _unitOfWork.Orders.FetchAllAsync();
            var hasOrders = orders.Any(o => o.CustomerId == id);

            if (hasOrders)
            {
                TempData["ErrorMessage"] = "Cannot delete customer that has orders. Please delete the orders first.";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Customers.Remove(customer);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Customer deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/Customers/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _unitOfWork.Customers.FindAsync(id.Value);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Sales/Customers/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            var customer = await _unitOfWork.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            customer.IsActive = false;

            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Customer deactivated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/Customers/QuickCreate
        public IActionResult QuickCreate()
        {
            return PartialView("_QuickCreate");
        }

        // POST: Sales/Customers/QuickCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickCreate(string name, string phone, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new { success = false, message = "Name is required" });
            }

            var customer = new Customer
            {
                Name = name,
                Phone = phone,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Customer added successfully",
                customerId = customer.Id,
                customerName = customer.Name
            });
        }
    

}
}
