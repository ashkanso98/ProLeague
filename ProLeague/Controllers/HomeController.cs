// ProLeague/Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using ProLeague.Application.Interfaces;

namespace ProLeague.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _homeService.GetHomeViewModelAsync();
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // ... منطق نمایش خطا
            return View();
        }
    }
}