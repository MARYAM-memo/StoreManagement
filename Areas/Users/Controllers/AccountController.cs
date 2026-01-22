using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Areas.Users.Models;
using StoreManagement.Areas.Users.ViewModels.AccountVM;

namespace StoreManagement.Areas.Users.Controllers
{
    [Area("Users")]
    public class AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager=signInManager;
        private readonly ILogger<AccountController> _logger=logger;

        // GET: Users/Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Users/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName ?? "",
                    model.Password ?? "",
                    model.RememberMe,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Update last login date
                    var user = await _userManager.FindByNameAsync(model.UserName ?? "");
                    if (user != null)
                    {
                        user.LastLoginDate = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                    }

                    // Check if password change is forced
                    if (user?.ForcePasswordChange == true)
                    {
                        return RedirectToAction("ChangePassword", new { area = "Users" });
                    }

                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError(string.Empty, "Account is locked. Please try again later.");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        // GET: Users/Account/Register
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Users/Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName ?? "",
                    LastName = model.LastName ?? "",
                    PhoneNumber = model.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    ForcePasswordChange = true // Force password change on first login
                };

                var result = await _userManager.CreateAsync(user, model.Password ?? "");

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Add to default role (if exists)
                    var defaultRole = "User";
                    if (await _userManager.GetRolesAsync(user) == null ||
                        !(await _userManager.GetRolesAsync(user)).Any())
                    {
                        await _userManager.AddToRoleAsync(user, defaultRole);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // POST: Users/Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // GET: Users/Account/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Users/Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangeAccountPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword ?? "",
                model.NewPassword ?? ""
            );

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            user.ForcePasswordChange = false;
            user.LastPasswordChangeDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User changed their password successfully.");
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // GET: Users/Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Users/Account/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileVM
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
                LastLoginDate = user.LastLoginDate,
                CreatedAt = user.CreatedAt
            };

            return View(model);
        }

        // POST: Users/Account/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.EmployeeId = model.EmployeeId;
            user.Department = model.Department;
            user.Position = model.Position;
            user.ProfilePicture = model.ProfilePicture;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Profile", model);
        }
    }
}
