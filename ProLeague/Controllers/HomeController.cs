// ProLeague/Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels;
using ProLeague.Infrastructure.Data;

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



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // ... منطق نمایش خطا
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        // POST: /Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // In a real application, you would send an email here using a service.
                // For now, we'll just show a success message.

                TempData["SuccessMessage"] = "پیام شما با موفقیت ارسال شد. سپاسگزاریم!";
                return RedirectToAction("Contact");
            }

            return View(model);
        }
    }
}