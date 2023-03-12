using HotelManager.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HotelManager.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
