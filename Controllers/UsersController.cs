using GoWebApplication.Db.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Users/
        public async Task<IActionResult> Index()
        {
            // Получаем список всех пользователей из БД через DbContext
            // Мы используем .ToListAsync(), чтобы EF Core выполнил SQL-запрос к MySQL
            var users = await _context.Users.ToListAsync();

            // Передаем список пользователей в представление (View)
            return View(users);
        }
    }
}
