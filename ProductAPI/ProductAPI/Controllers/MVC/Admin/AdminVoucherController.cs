using Microsoft.AspNetCore.Mvc;

namespace ProductAPI.Controllers.MVC.Admin
{
    public class AdminVoucherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
