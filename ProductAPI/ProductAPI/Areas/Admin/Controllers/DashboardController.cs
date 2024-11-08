using Microsoft.AspNetCore.Mvc;

namespace ProductAPI.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
