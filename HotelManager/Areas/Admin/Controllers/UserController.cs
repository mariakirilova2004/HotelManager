using HotelManager.Areas.Admin.Controllers;
using HotelManager.Core.Constants;
using HotelManager.Core.Models.User;
using HotelManager.Core.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManager.Areas.Admin.Controllers
{

    public class UserController : AdminController
    {
        private readonly IUserService users;
        private readonly IMemoryCache cache;
        private readonly ILogger logger;
        public UserController(IUserService _users,
                              IMemoryCache _cache,
                              ILogger<UserController> _logger)
        {
            this.users = _users;
            this.cache = _cache;
            this.logger = _logger;
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

        //Forget the data of the user but the id remains

        //[HttpPost]
        //public async Task<IActionResult> Forget(string Id)
        //{
        //    try
        //    {
        //        await this.users.Forget(Id);
        //        TempData[MessageConstant.SuccessMessage] = "Successfully deleted user";
        //    }
        //    catch (Exception)
        //    {
        //        TempData[MessageConstant.ErrorMessage] = "Unsuccessfully deleted user";
        //        this.logger.LogInformation("User {0} could not be deleted!", Id);
        //    }
        //    return RedirectToAction(nameof(All));
        //}
    }
}
