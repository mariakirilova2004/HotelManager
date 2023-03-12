//using HotelManager.Areas.Admin.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Caching.Memory;

//namespace HomeController.Areas.Admin.Controllers
//{
    
//    public class UserController : AdminController
//    {
//        private readonly IUserService users;
//        private readonly IMemoryCache cache;
//        private readonly ILogger logger;
//        public UserController(IUserService _users,
//                              IMemoryCache _cache,
//                              ILogger<UserController> _logger)
//        {
//            this.users = _users;
//            this.cache = _cache;
//            this.logger = _logger;
//        }

//        [Route("/Admin/User/All")]
//        public IActionResult All()
//        {
//            var users = this.cache.Get<IEnumerable<UserServiceModel>>("UsersCacheKey");
//            if(users == null)
//            {
//                users = this.users.All();

//                var cacheOptions = new MemoryCacheEntryOptions()
//                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

//                this.cache.Set("UsersCacheKey", users, cacheOptions);
//            }
//            return View(users);
//        }

//        //Forget the data of the user but the id remains

//        [HttpPost]
//        public async Task<IActionResult> Forget(string Id)
//        {
//            try
//            {
//                await this.users.Forget(Id);
//                TempData[MessageConstant.SuccessMessage] = "Successfully deleted user";
//            }
//            catch (Exception)
//            {
//                TempData[MessageConstant.ErrorMessage] = "Unsuccessfully deleted user";
//                this.logger.LogInformation("User {0} could not be deleted!", Id);
//            }
//            return RedirectToAction(nameof(All));
//        }
//    }
//}
