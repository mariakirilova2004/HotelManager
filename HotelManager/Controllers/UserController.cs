using Microsoft.AspNetCore.Mvc;

namespace HotelManager.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
