using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Areas.Users.Models;
using StoreManagement.Areas.Users.ViewModels.UserVM;

namespace StoreManagement.Areas.Users.Controllers
{
    [Area("Users")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UsersController> logger)
     : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly ILogger<UsersController> _logger = logger;

        // GET: Users/Users
        public async Task<IActionResult> Index(string searchTerm, string filter = "all")
        {
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentFilterType"] = filter;

            var users = _userManager.Users.AsQueryable();

            // Apply filters
            if (filter == "active")
                users = users.Where(u => u.IsActive);
            else if (filter == "inactive")
                users = users.Where(u => !u.IsActive);

            // Apply search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u =>
                    u.FirstName.Contains(searchTerm) ||
                    u.LastName.Contains(searchTerm) ||
                    (u.Email ?? "").Contains(searchTerm) ||
                    (u.UserName ?? "").Contains(searchTerm) ||
                    (u.EmployeeId ?? "").Contains(searchTerm)
                );
            }

            var userList = await users.ToListAsync();
            var userVMs = new List<UserVM>();

            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userVMs.Add(new UserVM
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName ?? "",
                    EmployeeId = user.EmployeeId,
                    Department = user.Department,
                    Position = user.Position,
                    ProfilePicture = user.ProfilePicture,
                    IsActive = user.IsActive,
                    ForcePasswordChange = user.ForcePasswordChange,
                    Roles = roles.ToList(),
                    LastLoginDate = user.LastLoginDate,
                    CreatedAt = user.CreatedAt
                });
            }

            return View(userVMs.OrderBy(u => u.LastName).ToList());
        }

        // GET: Users/Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleDetails = new List<ApplicationRole>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roleDetails.Add(role);
                }
            }

            ViewBag.UserRoles = roleDetails;

            return View(user);
        }

        // GET: Users/Users/Create
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.Where(r => !r.IsSystemRole).ToListAsync();

            var model = new CreateUserVM
            {
                AvailableRoles = roles
            };

            return View(model);
        }

        // POST: Users/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM model)
        {
            
            System.Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName ?? "",
                    LastName = model.LastName ?? "",
                    PhoneNumber = model.PhoneNumber,
                    EmployeeId = model.EmployeeId,
                    Department = model.Department,
                    Position = model.Position,
                    ProfilePicture = model.ProfilePicture,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ForcePasswordChange = true // Force password change on first login
                    
                };

                System.Console.WriteLine($"Creating user with username: {model.UserName}, email: {model.Email}");
                System.Console.WriteLine($"Password length: {model.Password?.Length}");
                var result = await _userManager.CreateAsync(user, model.Password ?? "");
                System.Console.WriteLine($"result.Succeeded: {result.Succeeded}");
                if (result.Succeeded)
                {
                    // Assign selected roles
                    if (model.SelectedRoles != null && model.SelectedRoles.Count != 0)
                    {
                        foreach (var roleName in model.SelectedRoles)
                        {
                            if (await _roleManager.RoleExistsAsync(roleName))
                            {
                                await _userManager.AddToRoleAsync(user, roleName);
                            }
                        }
                    }

                    TempData["SuccessMessage"] = $"User '{user.FullName}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                System.Console.WriteLine($"Number of errors: {result.Errors.Count()}");
                foreach (var error in result.Errors)
                {
                    System.Console.WriteLine($"error: {error}");
                    System.Console.WriteLine($"Error Code: {error.Code}");
                    System.Console.WriteLine($"Error Description: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                _logger.LogError("Failed to create user. Errors: {Errors}",
            string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // If we got this far, something failed; reload roles
            model.AvailableRoles = await _roleManager.Roles.Where(r => !r.IsSystemRole).ToListAsync();
            return View(model);
        }

        // GET: Users/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Where(r => !r.IsSystemRole).ToListAsync();

            var model = new EditUserVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                PhoneNumber = user.PhoneNumber,
                EmployeeId = user.EmployeeId,
                Department = user.Department,
                Position = user.Position,
                ProfilePicture = user.ProfilePicture,
                IsActive = user.IsActive,
                ForcePasswordChange = user.ForcePasswordChange,
                SelectedRoles = userRoles.ToList(),
                AvailableRoles = roles
            };

            return View(model);
        }

        // POST: Users/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                // Update user properties
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.PhoneNumber = model.PhoneNumber;
                user.EmployeeId = model.EmployeeId;
                user.Department = model.Department;
                user.Position = model.Position;
                user.ProfilePicture = model.ProfilePicture;
                user.IsActive = model.IsActive;
                user.ForcePasswordChange = model.ForcePasswordChange;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Update roles
                    var currentRoles = await _userManager.GetRolesAsync(user);

                    // Remove roles not in selected list
                    var rolesToRemove = currentRoles.Except(model.SelectedRoles);
                    foreach (var role in rolesToRemove)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }

                    // Add new roles
                    var rolesToAdd = model.SelectedRoles.Except(currentRoles);
                    foreach (var role in rolesToAdd)
                    {
                        if (await _roleManager.RoleExistsAsync(role))
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }

                    TempData["SuccessMessage"] = $"User '{user.FullName}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed; reload roles
            model.AvailableRoles = await _roleManager.Roles.Where(r => !r.IsSystemRole).ToListAsync();
            return View(model);
        }

        // GET: Users/Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Prevent deleting your own account
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: Users/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Prevent deleting your own account
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User '{user.FullName}' deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = $"Error deleting user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Users/ChangePassword/5
        public async Task<IActionResult> ChangePassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordVM
            {
                UserId = user.Id
            };

            ViewBag.UserName = user.UserName;
            ViewBag.FullName = user.FullName;

            return View(model);
        }

        // POST: Users/Users/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.UserId == null)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId!);

            if (user == null)
            {
                return NotFound();
            }

            // Check if current password is correct
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.CurrentPassword ?? "");

            if (!isPasswordCorrect)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                return View(model);
            }

            // Change password
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword ?? "", model.NewPassword ?? "");

            if (result.Succeeded)
            {
                user.LastPasswordChangeDate = DateTime.UtcNow;
                user.ForcePasswordChange = false;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction(nameof(Details), new { id = model.UserId });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Users/Users/ResetPassword/5
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordVM
            {
                UserId = user.Id,
                UserName = user.UserName
            };

            ViewBag.FullName = user.FullName;

            return View(model);
        }

        // POST: Users/Users/ResetPassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.UserId == null)
            {
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId!);

            if (user == null)
            {
                return NotFound();
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset password
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword ?? "");

            if (result.Succeeded)
            {
                user.LastPasswordChangeDate = DateTime.UtcNow;
                user.ForcePasswordChange = true; // Force user to change password on next login
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = $"Password reset successfully for {user.UserName}!";
                return RedirectToAction(nameof(Details), new { id = model.UserId });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Users/Users/Deactivate/5
        public async Task<IActionResult> Deactivate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Prevent deactivating your own account
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["ErrorMessage"] = "You cannot deactivate your own account!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: Users/Users/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Prevent deactivating your own account
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["ErrorMessage"] = "You cannot deactivate your own account!";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User '{user.FullName}' deactivated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = $"Error deactivating user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Users/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User '{user.FullName}' activated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = $"Error activating user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX: Get user details
        [HttpGet]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Json(new
            {
                success = true,
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                userName = user.UserName,
                isActive = user.IsActive,
                lastLogin = user.LastLoginDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never",
                roles = roles
            });
        }

    }
}
