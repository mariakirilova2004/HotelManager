using HotelManager.Extensions;
using HotelManager.Infrastructure.Data.Еntities.Account;
using HotelManager.Core.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WeVolunteer.Core.Constants;

namespace HotelManager.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ILogger logger;
        private readonly IMemoryCache cache;

        public UserController(
               UserManager<User> _userManager,
               SignInManager<User> _signInManager,
               ILogger _logger,
               IMemoryCache _cache)
        {
            this.userManager = _userManager;
            this.signInManager = _signInManager;
            this.logger = _logger;
            this.cache = _cache;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                this.logger.LogInformation("User {0} tried to login again!", this.User.Id());
                return RedirectToAction("Index", "Home");
            }

            var model = new UserLoginViewModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                TempData[MessageConstant.WarningMessage] = "You have already logged in";
                this.logger.LogInformation("User {0} tried to login again!", this.User.Id());
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                TempData[MessageConstant.ErrorMessage] = "Invalid login";
                return View(model);
            }

            var user = userManager.Users.Where(u => u.Email == model.Email).FirstOrDefault();

            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    this.cache.Remove("UsersCacheKey");
                    TempData[MessageConstant.SuccessMessage] = "You have successfully logged in";
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login");
            TempData[MessageConstant.ErrorMessage] = "Invalid login";

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            TempData[MessageConstant.SuccessMessage] = "You have successfully logged out";
            return RedirectToAction("Index", "Home");
        }
    }
}
