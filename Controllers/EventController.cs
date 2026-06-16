using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Repositories;
using GoWeb.Service;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Security.Claims;

namespace GoWeb.Areas.Manage.Controllers
{
    public class EventController : Controller
    {
        private readonly ICityService cityService;
        private readonly IEventRepository eventRepository;
        private readonly IMapper mapper;
        private readonly IStatusEvent statusEventRepository;
        private readonly IEventTypeService eventTypeService;
        private readonly IEventService eventService;
        private readonly IUserRepository userRepository;
        private readonly IUserEvent userEvent;
        private readonly IUserEventService userEventService;
        public EventController(ICityService cityService, IEventRepository eventRepository,
                               IMapper mapper, IStatusEvent statusEventRepository, IEventTypeService eventTypeService,
                               IEventService eventService, IUserRepository userRepository, IUserEvent userEvent, IUserEventService userEventService)
        {
            this.userEvent = userEvent;
            this.cityService = cityService;
            this.eventRepository = eventRepository;
            this.mapper = mapper;
            this.statusEventRepository = statusEventRepository;
            this.eventTypeService = eventTypeService;
            this.eventService = eventService;
            this.userRepository = userRepository;
            this.userEvent = userEvent;
            this.userEventService = userEventService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(EventFilterViewModel filter)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), new
                {
                    selectedCity = filter.SelectedCity,
                    selectedTypeEvent = filter.SelectedTypeEvent
                });
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Index(int? selectedCity, int? selectedTypeEvent)
        {
          
            if (!selectedCity.HasValue && User.Identity.IsAuthenticated)
            {
                var cityId =int.Parse(User.FindFirstValue("idCity")); 
                selectedCity = cityId;
            }
            var model = await eventService.GetFilterEvents(selectedCity, selectedTypeEvent);
            if (selectedCity.HasValue)
            {
                var listCity = await cityService.GetAllAsync();
                var city = listCity.FirstOrDefault(c => c.Id == selectedCity);
                if (city != null)
                {
                    model.Filter.SelectedCityCoordinate = new double[] { city.LocationLongitude, city.LocationLatitude };
                    model.result = await eventService.GetFilteredEventsAsync(model.Filter);
                }
                else
                {
                    TempData["ErrorMessage"] = "Выбран несуществующий город";
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Event(int id, string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
           {
               ViewBag.ReturnUrl = returnUrl;
           }
           var ev = await userEventService.GetEventsWithUserAsync(id);
           if(ev!=null)
            {
                return View(ev);
            }
            TempData["ErrorMessage"] = "Ошибка, данного события не существует!";
            return RedirectToAction("Event", "Index", new {area=""});
        }





    }
}
