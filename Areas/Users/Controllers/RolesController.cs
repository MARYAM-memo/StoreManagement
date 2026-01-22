using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Areas.Users.Models;
using StoreManagement.Areas.Users.ViewModels.RoleVM;

namespace StoreManagement.Areas.Users.Controllers
{
    [Area("Users")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RolesController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RolesController> logger) : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<RolesController> _logger = logger;

        // GET: Users/Roles
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var roles = _roleManager.Roles.AsQueryable();

            // Apply search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                roles = roles.Where(r =>
                    (r.Name??"").Contains(searchTerm) ||
                    (r.Description??"").Contains(searchTerm)
                );
            }

            var roleList = await roles.ToListAsync();
            var roleVMs = new List<RoleVM>();

            foreach (var role in roleList)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name??"");

                roleVMs.Add(new RoleVM
                {
                    Id = role.Id,
                    Name = role.Name??"",
                    Description = role.Description,
                    IsSystemRole = role.IsSystemRole,
                    UserCount = usersInRole.Count,
                    CreatedAt = role.CreatedAt,
                    CanManageUsers = role.CanManageUsers,
                    CanManageRoles = role.CanManageRoles,
                    CanManageProducts = role.CanManageProducts,
                    CanManageCategories = role.CanManageCategories,
                    CanManageBrands = role.CanManageBrands,
                    CanManageCustomers = role.CanManageCustomers,
                    CanManageOrders = role.CanManageOrders,
                    CanManageSuppliers = role.CanManageSuppliers,
                    CanManageInventory = role.CanManageInventory,
                    CanViewReports = role.CanViewReports,
                    CanManageSettings = role.CanManageSettings
                });
            }

            return View(roleVMs.OrderBy(r => r.Name).ToList());
        }

        // GET: Users/Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            // Get users in this role
            var users = await _userManager.GetUsersInRoleAsync(role.Name??"");

            ViewBag.UsersInRole = users;

            return View(role);
        }

        // GET: Users/Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleVM model)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsSystemRole = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name,
                    CanManageUsers = model.CanManageUsers,
                    CanManageRoles = model.CanManageRoles,
                    CanManageProducts = model.CanManageProducts,
                    CanManageCategories = model.CanManageCategories,
                    CanManageBrands = model.CanManageBrands,
                    CanManageCustomers = model.CanManageCustomers,
                    CanManageOrders = model.CanManageOrders,
                    CanManageSuppliers = model.CanManageSuppliers,
                    CanManageInventory = model.CanManageInventory,
                    CanViewReports = model.CanViewReports,
                    CanManageSettings = model.CanManageSettings
                };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Role '{role.Name}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: Users/Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            // Prevent editing system roles
            if (role.IsSystemRole)
            {
                TempData["ErrorMessage"] = "System roles cannot be edited!";
                return RedirectToAction(nameof(Index));
            }

            var model = new RoleVM
            {
                Id = role.Id,
                Name = role.Name??"",
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                CanManageUsers = role.CanManageUsers,
                CanManageRoles = role.CanManageRoles,
                CanManageProducts = role.CanManageProducts,
                CanManageCategories = role.CanManageCategories,
                CanManageBrands = role.CanManageBrands,
                CanManageCustomers = role.CanManageCustomers,
                CanManageOrders = role.CanManageOrders,
                CanManageSuppliers = role.CanManageSuppliers,
                CanManageInventory = role.CanManageInventory,
                CanViewReports = role.CanViewReports,
                CanManageSettings = role.CanManageSettings// تكملة Areas/Users/Controllers/RolesController.cs
            };

            return View(model);
        }

        // POST: Users/Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RoleVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);

                if (role == null)
                {
                    return NotFound();
                }

                // Prevent editing system roles
                if (role.IsSystemRole)
                {
                    TempData["ErrorMessage"] = "System roles cannot be edited!";
                    return RedirectToAction(nameof(Index));
                }

                // Update role properties
                role.Name = model.Name;
                role.Description = model.Description;
                role.CanManageUsers = model.CanManageUsers;
                role.CanManageRoles = model.CanManageRoles;
                role.CanManageProducts = model.CanManageProducts;
                role.CanManageCategories = model.CanManageCategories;
                role.CanManageBrands = model.CanManageBrands;
                role.CanManageCustomers = model.CanManageCustomers;
                role.CanManageOrders = model.CanManageOrders;
                role.CanManageSuppliers = model.CanManageSuppliers;
                role.CanManageInventory = model.CanManageInventory;
                role.CanViewReports = model.CanViewReports;
                role.CanManageSettings = model.CanManageSettings;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Role '{role.Name}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: Users/Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            // Prevent deleting system roles
            if (role.IsSystemRole)
            {
                TempData["ErrorMessage"] = "System roles cannot be deleted!";
                return RedirectToAction(nameof(Index));
            }

            // Check if role has users
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name??"");

            ViewBag.UserCount = usersInRole.Count;

            return View(role);
        }

        // POST: Users/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            // Prevent deleting system roles
            if (role.IsSystemRole)
            {
                TempData["ErrorMessage"] = "System roles cannot be deleted!";
                return RedirectToAction(nameof(Index));
            }

            // Check if role has users
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name??"");

            if (usersInRole.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete role '{role.Name}' because it has {usersInRole.Count} users assigned. Please reassign users first.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role '{role.Name}' deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = $"Error deleting role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Roles/ManageUsers/5
        public async Task<IActionResult> ManageUsers(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name??"");
            var allUsers = await _userManager.Users.ToListAsync();

            var model = new UserRoleVM
            {
                AvailableRoles = [role.Name??""]
            };

            ViewBag.RoleName = role.Name;
            ViewBag.RoleId = role.Id;
            ViewBag.UsersInRole = usersInRole;
            ViewBag.AllUsers = allUsers;

            return View(model);
        }

        // POST: Users/Roles/AddUserToRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToRole(string roleId, string userId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var user = await _userManager.FindByIdAsync(userId);

            if (role == null || user == null)
            {
                return Json(new { success = false, message = "Role or user not found" });
            }

            var result = await _userManager.AddToRoleAsync(user, role.Name??"");

            if (result.Succeeded)
            {
                return Json(new { success = true, message = $"User added to {role.Name} role successfully!" });
            }

            return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        // POST: Users/Roles/RemoveUserFromRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserFromRole(string roleId, string userId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var user = await _userManager.FindByIdAsync(userId);

            if (role == null || user == null)
            {
                return Json(new { success = false, message = "Role or user not found" });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name??"");

            if (result.Succeeded)
            {
                return Json(new { success = true, message = $"User removed from {role.Name} role successfully!" });
            }

            return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        // AJAX: Get role details
        [HttpGet]
        public async Task<IActionResult> GetRoleDetails(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return Json(new { success = false, message = "Role not found" });
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name??"");

            return Json(new
            {
                success = true,
                id = role.Id,
                name = role.Name,
                description = role.Description,
                isSystemRole = role.IsSystemRole,
                userCount = usersInRole.Count,
                createdDate = role.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                permissions = new
                {
                    canManageUsers = role.CanManageUsers,
                    canManageRoles = role.CanManageRoles,
                    canManageProducts = role.CanManageProducts,
                    canManageCategories = role.CanManageCategories,
                    canManageBrands = role.CanManageBrands,
                    canManageCustomers = role.CanManageCustomers,
                    canManageOrders = role.CanManageOrders,
                    canManageSuppliers = role.CanManageSuppliers,
                    canManageInventory = role.CanManageInventory,
                    canViewReports = role.CanViewReports,
                    canManageSettings = role.CanManageSettings
                }
            });
        }


    }
}
