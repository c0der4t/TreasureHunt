using Microsoft.AspNetCore.Mvc;

namespace TreasureHuntFrontend.Controllers
{
    public class HintController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
