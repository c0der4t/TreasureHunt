using Microsoft.AspNetCore.Mvc;

namespace TreasureHuntFrontend.Controllers
{
    public class FindController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
