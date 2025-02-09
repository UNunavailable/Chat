using Microsoft.AspNetCore.Mvc;

namespace HistoryClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}