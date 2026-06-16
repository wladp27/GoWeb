using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Repositories;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
  //  [Authorize(Roles = "Администратор")]
    public class EventTypeController : Controller
    {
        public readonly IEventTypeService eventTypeService;
        public readonly IMapper mapper;
        public EventTypeController(IEventTypeService eventTypeService, IMapper mapper)
        {
            this.eventTypeService = eventTypeService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var listEventType = await eventTypeService.GetAllAsync();
            var listEventTypeView = mapper.Map<List<EventTypeViewMode>>(listEventType); 
            return View(listEventTypeView);
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventTypeViewMode eventTypeView)
        {
            if (ModelState.IsValid)
            {
                await eventTypeService.AddAsync(eventTypeView);
                return RedirectToAction(nameof(Index));
            }
            return View(eventTypeView);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var eventType = await eventTypeService.GetByIdAsync(id);
            if (eventType == null)
            {
                return NotFound();
            }
            else
            {
                var eventTypeView = mapper.Map<EventTypeViewMode>(eventType);
                return View(eventTypeView);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventTypeViewMode eventTypeView)
        {
            if (ModelState.IsValid)
            {
                var existingEventType = await eventTypeService.GetByIdAsync(eventTypeView.Id.Value);
                if (existingEventType == null)
                {
                    ModelState.AddModelError(string.Empty, "Тип события не найден для обновления. Возможно,он был удален.");
                    return View(eventTypeView);
                }
                await eventTypeService.Update(existingEventType);
                return RedirectToAction(nameof(Index));
            }
            return View(eventTypeView);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var existingEventType = await eventTypeService.GetByIdAsync(id);
            if (existingEventType == null)
            {
                return NotFound();
            }
            return View(existingEventType);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existingCity = await eventTypeService.GetByIdAsync(id);
            if (existingCity != null)
            {
               await eventTypeService.DeleteAsync(existingCity);
            }
            return RedirectToAction(nameof(Index));
        }



    }




}
