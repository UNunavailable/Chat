using Microsoft.AspNetCore.Mvc;

namespace SenderClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
