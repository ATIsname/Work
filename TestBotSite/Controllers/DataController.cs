using Microsoft.AspNetCore.Mvc;

namespace TestBotSite.Controllers
{
    public class DataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
