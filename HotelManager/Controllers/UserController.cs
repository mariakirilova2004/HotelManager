using Microsoft.AspNetCore.Mvc;

namespace HotelManager.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
