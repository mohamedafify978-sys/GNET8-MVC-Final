using System.Diagnostics;
using System.Threading.Tasks;
using GYMSystem.BLL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC1.Models;

namespace MVC1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IAnalyticsService analyticsService;

        public HomeController(ILogger<HomeController> logger ,IAnalyticsService analyticsService)
        {
            this.logger = logger;
            this.analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await analyticsService.GetDataAsync();
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}
