using Microsoft.AspNetCore.Mvc;

namespace FESTA.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult QuienesSomos()
        {
            return View();
        }
    }
}

