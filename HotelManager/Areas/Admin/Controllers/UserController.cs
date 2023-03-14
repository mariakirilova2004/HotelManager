using Ganss.Xss;
using HotelManager.Areas.Admin.Controllers;
using HotelManager.Core.Constants;
using HotelManager.Core.Models.User;
using HotelManager.Core.Services.User;
using HotelManager.Extensions;
using HotelManager.Infrastructure.Data.Еntities.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManager.Areas.Admin.Controllers
{

    public class UserController : AdminController
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IUserService users;
        private readonly IMemoryCache cache;
        private readonly ILogger logger;
        public UserController(IUserService _users,
                              IMemoryCache _cache,
                              ILogger<UserController> _logger,
                              UserManager<User> _userManager,
                              SignInManager<User> _signInManager)
        {
            this.users = _users;
            this.cache = _cache;
            this.logger = _logger;
            this.userManager = _userManager;
            this.signInManager = _signInManager;
        }

        [Route("/Admin/User/AllUsers")]
        public IActionResult All([FromQuery] AllUsersQueryModel query)
        {
            var users = this.cache.Get<AllUsersQueryModel>("UsersCacheKey");
            if (users == null)
            {
                var queryResult = this.users.All(
                    query.SearchTerm,
                    query.SearchTermOn,
                    query.CurrentPage,
                    query.UsersPerPage);

                query.TotalUsersCount = queryResult.TotalUsersCount;
                query.Users = queryResult.Users;

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

                this.cache.Set("UsersCacheKey", users, cacheOptions);
            }
            return View(query);
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (!User.IsAdmin())
            {
                TempData[MessageConstant.WarningMessage] = "You cannot add Users!";
                this.logger.LogInformation("User {0} tried to add user, but they are not Admin!", this.User.Id());
                return RedirectToAction("All", "User");
            }
            var model = new AddUserFormModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserFormModel model)
        {
            var sanitalizer = new HtmlSanitizer();

            if (!ModelState.IsValid)
            {
                TempData[MessageConstant.ErrorMessage] = "Invalid add";
                return View(model);
            }

            if (userManager.Users.Any(u => u.Email == model.Email))
            {
                TempData[MessageConstant.ErrorMessage] = "Тhere is a user with this email";
                return View(model);
            }

            if (userManager.Users.Any(u => u.PhoneNumber == model.PhoneNumber))
            {
                TempData[MessageConstant.ErrorMessage] = "Тhere is a user with this Phone Number";
                return View(model);
            }

            var user = new User()
            {
                UserName = sanitalizer.Sanitize(model.UserName),
                FirstName = sanitalizer.Sanitize(model.FirstName),
                MiddleName = sanitalizer.Sanitize(model.MiddleName),
                LastName = sanitalizer.Sanitize(model.LastName),
                EGN = sanitalizer.Sanitize(model.EGN),
                Email = sanitalizer.Sanitize(model.Email),
                PhoneNumber = sanitalizer.Sanitize(model.PhoneNumber),
                HiringDate = model.HiringDate,
                IsActive = true,
                DismissionDate = null,
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                TempData[MessageConstant.SuccessMessage] = "Your account has been successfully registered";
                return RedirectToAction("All", "User");
            }

            foreach (var item in result.Errors)
            {
                TempData[MessageConstant.ErrorMessage] = item.Description.ToString();
            }

            return View(model);
        }

        //Forget the data of the user but the id remains

        [HttpPost]
        public async Task<IActionResult> Forget(string Id)
        {
            try
            {
                await this.users.Forget(Id);
                TempData[MessageConstant.SuccessMessage] = "Successfully deleted user";
            }
            catch (Exception)
            {
                TempData[MessageConstant.ErrorMessage] = "Unsuccessfully deleted user";
                this.logger.LogInformation("User {0} could not be deleted!", Id);
            }
            return RedirectToAction(nameof(All));
        }
    }
}
