using GoWeb.Interfaces;
using GoWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(Roles = "Администратор")]
    public class EventController : Controller
    {
        private readonly IEventService eventService;
        public EventController(IEventService eventService)
        {
            this.eventService = eventService;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eventToDelete = await eventService.GetByIdAsync(id);
            if (eventToDelete != null)
            {
                var isDeleted = await eventService.DeleteBuIdAsync(id);
                if (isDeleted)
                {
                    TempData["SuccessMessage"] = $"Событие '{eventToDelete.Id}' успешно удалено.";
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = $"Ошибка удаления, у данного события id={id} есть пользователи";
                }
            }
            TempData["ErrorMessage"] = $"Ошибка: Событие с ID {id} не найдено.";
            return View();
        }


        public async Task<IActionResult> DeleteFor(int id, int? SelectedTypeEvent, int? SelectedCity)
        {

            var eventToDelete = await eventService.GetByIdAsync(id);
            if (eventToDelete != null)
            {
                var isDeleted = await eventService.DeleteBuIdAsync(id);
                if (isDeleted)
                {
                    TempData["SuccessMessage"] = $"Событие '{eventToDelete.Id}' успешно удалено.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Ошибка удаления, у данного события id={id} есть пользователи";
                }
            }
            else
            {
                TempData["ErrorMessage"] = $"Ошибка: Событие с ID {id} не найдено.";
            }

        
            return RedirectToAction("Index", "Event", new {area="Manage",
                                                           SelectedTypeEvent = SelectedTypeEvent,
                                                           SelectedCity= SelectedCity});
        }
    }
}
