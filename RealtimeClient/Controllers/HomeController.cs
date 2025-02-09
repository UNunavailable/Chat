using Microsoft.AspNetCore.Mvc;

namespace RealtimeClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}