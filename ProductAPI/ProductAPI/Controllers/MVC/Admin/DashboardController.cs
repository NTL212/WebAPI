using Microsoft.AspNetCore.Mvc;

namespace ProductAPI.Controllers.MVC.Admin
{
	public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
