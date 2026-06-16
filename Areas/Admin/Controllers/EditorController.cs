using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Администратор")]
    public class EditorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
