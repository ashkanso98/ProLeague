using Microsoft.AspNetCore.Mvc;
using ProLeague.Application.Interfaces;
using System.Threading.Tasks;

namespace ProLeague.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        // GET: Player/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // از سرویس برای دریافت بازیکن به همراه اطلاعات تیمش استفاده می‌کنیم
            var player = await _playerService.GetPlayerWithTeamDetailsAsync(id);

            if (player == null)
            {
                return NotFound(); // اگر بازیکن پیدا نشد، خطای ۴۰۴ برمی‌گرداند
            }

            return View(player); // بازیکن معتبر به ویو ارسال می‌شود
        }
    }
}