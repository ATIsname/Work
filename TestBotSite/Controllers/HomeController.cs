using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TestBotSite.Models;

namespace TestBotSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string message)
        {
            return View("Index", message);
        }

        public async Task<IActionResult> Tests(string email, string password)
        {
            Admin admin = await Admin.GetAdmin(email, password);
            if (admin != null)
                return RedirectToAction(nameof(Index), "Test");
            else
                return View("Index", "Wrong email or password");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return RedirectToAction(nameof(Index), nameof(Error));
        }
    }
}
