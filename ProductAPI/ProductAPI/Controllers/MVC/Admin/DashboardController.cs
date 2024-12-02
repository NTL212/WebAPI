using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using ProductAPI.Services;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class DashboardController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public DashboardController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }
        public async Task<IActionResult> Index()
        {
            var dashboardVm  = await _statisticsService.GetDashboardVm();
            return View(dashboardVm);
        }

       
    }
}
