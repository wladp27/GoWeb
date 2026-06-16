using AutoMapper;

using GoWeb.Commands.Event;
using GoWeb.Filters;
using GoWeb.Filters.Authorization;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Repositories;
using GoWeb.Сonstants;
using GoWebApplication.Db.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Security.Claims;

namespace GoWeb.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class EventsController : Controller
    {
        private readonly ICityService cityService;
        private readonly IMapper mapper;
        private readonly IStatusEventService statusEventService;
        private readonly IEventTypeService eventTypeService;
        private readonly IEventService eventService;
        private readonly IUserRepository userRepository;
        private readonly ICommandQueue commandQueue;
        private readonly ILocationRepository locationRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IAuthorizationService authorizationService;
        private readonly IRatingRepository ratingRepository;
        public EventsController(ICityService cityService, IAuthorizationService authorizationService, IRatingRepository ratingRepository,
                               IMapper mapper, IStatusEventService statusEventService, IEventTypeService eventTypeService,
                               IEventService eventService, IUserRepository userRepository, ICommandQueue commandQueue, ILocationRepository locationRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.cityService = cityService;
            this.mapper = mapper;
            this.statusEventService = statusEventService;
            this.eventTypeService = eventTypeService;
            this.eventService = eventService;
            this.userRepository = userRepository;
            this.commandQueue = commandQueue;
            this.locationRepository = locationRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.authorizationService = authorizationService;
            this.ratingRepository = ratingRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int? selectedCity, int? selectedTypeEvent)
        {
            var listCity = await cityService.GetAllAsync();
            var listTypeEvents = await eventTypeService.GetAllAsync();
            if (User.Identity.IsAuthenticated)
            {

                var cityUser = await userRepository.GetByIdWithCityAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (cityUser != null)
                {
                    selectedCity = cityUser;
                }
            }
            var filter = new EventFilterViewModel
            {

                Cities = listCity.Select(c => new SelectListItem
                {
                    Text = c.NameCity,
                    Value = c.Id.ToString()
                }).ToList(),
                TypeEvents = listTypeEvents.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Id.ToString()
                }).ToList(),
                SelectedCity = selectedCity,
                SelectedTypeEvent = selectedTypeEvent
            };
            if (selectedCity != null)
            {
                var eventF = new EventIndexViewModel { Filter = filter };
                var listEvent = await eventService.GetFilteredEventsAsync(filter);
                eventF.result = listEvent;
                return View(eventF);
            }

            return View(new EventIndexViewModel { Filter = filter });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(EventFilterViewModel filter)
        {
            var listCity = await cityService.GetAllAsync();
            var Cities = listCity.Select(c => new SelectListItem
            {
                Text = c.NameCity,
                Value = c.Id.ToString()
            }).ToList();
            var listTypeEvents = await eventTypeService.GetAllAsync();
            var TypeEvents = listTypeEvents.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.Id.ToString()
            }).ToList();

            filter.Cities = Cities;
            filter.TypeEvents = TypeEvents;


            if (ModelState.IsValid)
            {
                var eventF = new EventIndexViewModel { Filter = filter };
                var listEvent = await eventService.GetFilteredEventsAsync(filter);
                eventF.result = listEvent;
                return View(eventF);
            }
            var eventFiltered = new EventIndexViewModel { Filter = filter };
            return View(eventFiltered);
        }




        //   [Authorize(Roles = "Администратор,Организатор")]
        public async Task<IActionResult> Create()
        {
            var listCity = await cityService.GetAllAsync();
            //var listStatus = await statusEventService.GetAllAsync();
            var listEventType = await eventTypeService.GetAllAsync();
            var selectListCity = listCity.Select(c => new SelectListItem
            {
                Text = c.NameCity,
                Value = c.Id.ToString()
            }).ToList();

            var selectListTypeEvent = listEventType.Select(ty =>
                                                        new SelectListItem
                                                        {
                                                            Text = ty.Name,
                                                            Value = ty.Id.ToString()
                                                        }).ToList();

            var selectListStatusEvent = new List<SelectListItem> {
                                                        new SelectListItem
                                                        {
                                                            Text = "Опубиковано",
                                                            Value = ((int)StatusEventConts.Published).ToString()
                                                        },
                                                        new SelectListItem
                                                        {
                                                            Text = "Черновик",
                                                            Value = ((int)StatusEventConts.Draft).ToString()
                                                        },
                                                        };


            var eventView = new EventCreateUpdateViewModel
            {
                Location = new LocationViewModel() { Cities = selectListCity },
                StatusEvents = selectListStatusEvent,
                EventTypes = selectListTypeEvent,
            };
            return View(eventView);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Администратор,Организатор")]
        public async Task<IActionResult> Create(EventCreateUpdateViewModel eventView)
        {
            await ValidateLocationAndImages(eventView);
            if (ModelState.IsValid)
            {
                WriterImages(eventView);
                var eventDb = mapper.Map<Event>(eventView);
                var idEvent = await eventService.AddAsync(eventDb);
                commandQueue.Enqueue(new CheckingСancelEventCommand(idEvent, eventView.EndTime), eventView.EndTime.AddHours(-(double)Timings.CanceledTime));
                return RedirectToAction(nameof(GoWeb.Areas.Manage.Controllers.EventController.Event),
                                        nameof(GoWeb.Areas.Manage.Controllers.EventController).Replace("Controller", ""), new { area = "", id = idEvent });
            }

            var listCity = await cityService.GetAllAsync();
            var listEventType = await eventTypeService.GetAllAsync();
            var selectListCity = listCity.Select(c => new SelectListItem
            {
                Text = c.NameCity,
                Value = c.Id.ToString()
            }).ToList();

            var selectListTypeEvent = listEventType.Select(ty =>
                                                        new SelectListItem
                                                        {
                                                            Text = ty.Name,
                                                            Value = ty.Id.ToString()
                                                        }).ToList();

            var selectListStatusEvent = new List<SelectListItem>
                                                        {
                                                        new SelectListItem
                                                        {
                                                            Text = "Опубиковано",
                                                            Value = ((int)StatusEventConts.Published).ToString()
                                                        },
                                                        new SelectListItem
                                                        {
                                                            Text = "Черновик",
                                                            Value = ((int)StatusEventConts.Draft).ToString()
                                                        },
                                                        };

            eventView.Location ??= new LocationViewModel();
            eventView.Location.Cities = selectListCity;
            eventView.StatusEvents = selectListStatusEvent;
            eventView.EventTypes = selectListTypeEvent;
            return View(eventView);
        }

        [TypeFilter(typeof(EnsureEventExists))]
        public async Task<IActionResult> SetRatingUser(string userName, int idEvent, int value) // Добавить проверку на существования пользователя
        {
            var ev = HttpContext.Items[typeof(Event)] as EventSummaryViewModel;
            var access = await authorizationService.AuthorizeAsync(User, ev, new OrginizerOrAdminRequirement());
            if (access.Succeeded)
            {
                var existRating = await ratingRepository.GetByIdAsync(userName, ev.EventTypeId);
                if (existRating != null)
                {
                    existRating.Value = value;
                    var resultUpdate =  await ratingRepository.UpdateAsync(existRating);
                    if (resultUpdate) TempData["SuccessMessage"] = "Рейтинг обновлен";
                    else TempData["ErrorMessage"] = "не удалось обновить рейтинг";
                }
                else
                {
                    var resultSet = await ratingRepository.AddAsync(new Rating { UserName = userName,EventTypeId = ev.EventTypeId,Value=value});
                    if (resultSet) TempData["SuccessMessage"] = "Рейтинг установлен";
                    else TempData["ErrorMessage"] = "Не удалось установить рейтинг";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка! Отказано в доступе";
            }
            return RedirectToAction(nameof(GoWeb.Areas.Manage.Controllers.EventController.Event),
                                        nameof(GoWeb.Areas.Manage.Controllers.EventController).Replace("Controller", ""), new { area = "", id = idEvent });
        }



        [TypeFilter(typeof(EnsureEventExists))]
        public async Task<IActionResult> UpdateStatusEvent(int idEvent, int idStatusEvent)
        {
            var ev = HttpContext.Items[typeof(Event)] as EventSummaryViewModel; 
            var StatusListOrganizer = new List<int>() { (int)StatusEventConts.Cancelled, (int)StatusEventConts.Draft };
            var access = await authorizationService.AuthorizeAsync(User, ev, new OrginizerOrAdminRequirement());
            if (access.Succeeded)
            {
                if (Enum.IsDefined(typeof(StatusEventConts), idStatusEvent))
                {
                    if (!StatusListOrganizer.Contains(idStatusEvent))
                    {
                        TempData["ErrorMessage"] = "Ошибка! Отказано в доступе";
                        return RedirectToAction(nameof(GoWeb.Areas.Manage.Controllers.EventController.Event),
                                        nameof(GoWeb.Areas.Manage.Controllers.EventController).Replace("Controller", ""), new { area = "", id = idEvent });
                    }
                    var result = await eventService.UpdateStatusEvent(idEvent, (StatusEventConts)idStatusEvent);
                    if (result)
                         TempData["SuccessMessage"] = "Статус события изменен";
                    else
                         TempData["ErrorMessage"] = "Ошибка, статус события не изменен";
                    
                    return RedirectToAction(nameof(GoWeb.Areas.Manage.Controllers.EventController.Event),
                                        nameof(GoWeb.Areas.Manage.Controllers.EventController).Replace("Controller", ""), new { area = "", id = idEvent });
                }
                else
                    TempData["ErrorMessage"] = "Ошибка! Данного типа статуса не сущесвует";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка! Отказано в доступе";
            }

            return RedirectToAction(nameof(GoWeb.Areas.Manage.Controllers.EventController.Event),
                                        nameof(GoWeb.Areas.Manage.Controllers.EventController).Replace("Controller", ""), new { area = "", id = idEvent });
        }




        [HttpGet]
        public async Task<IActionResult> GetUsers(string userName)
        {

            var users = await userRepository.GetUsers(userName);
            return Json(users.Select(u => new { UserName = u.UserName, Id = u.Id }).ToList());
        }

        public async Task<IActionResult>  GetLocations(string address,int idCity)
        {
            var locations = await locationRepository.GetLocations(address,idCity);
            return Json(locations.Select(l=>new {address= l.addrres, id=l.idLocation }));
        }


        [HttpGet]
        public async Task<IActionResult>  CheckLocation(string newAddress, int idCity)
        {
            
            return Json(await locationRepository.ExistsAddress(newAddress, idCity));
        }

        public async void  WriterImages(EventCreateUpdateViewModel eventView)
        {
                if (eventView.Location != null && eventView.Location.imagesLocation != null && eventView.Location.imagesLocation.Count > 0)
                {
                    string uploadsLocationFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "location");
                    if (!Directory.Exists(uploadsLocationFolder))
                    {
                        Directory.CreateDirectory(uploadsLocationFolder);
                    }
                    eventView.Location.imagesPaths = new List<string>();
                    foreach (var image in eventView.Location.imagesLocation)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName).ToLower();
                        string filePath = Path.Combine(uploadsLocationFolder, uniqueFileName);
                        eventView.Location.imagesPaths.Add(uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }
                    }
                }
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "events");
                if (eventView.ImageFile != null)
                {
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var extension = Path.GetExtension(eventView.ImageFile.FileName).ToLower();
                    var uniqueFileName = Guid.NewGuid().ToString() + extension;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    eventView.ImagePath = uniqueFileName;
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await eventView.ImageFile.CopyToAsync(fileStream);
                    }
                }
        }
        private async Task ValidateLocationAndImages(EventCreateUpdateViewModel eventView)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            if (eventView.LocationId != null)
            {
                if (!await locationRepository.ExistsAddress(eventView.LocationId.Value))
                    ModelState.AddModelError("AddressInput", "Данной локации в базе данных нет, либо она была удалена");
                var extension = String.Empty;
                if (eventView.ImageFile != null)
                {
                    extension = Path.GetExtension(eventView.ImageFile.FileName).ToLower();
                }
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageFile", $"Формат {extension} не поддерживается");
                }
                eventView.Location = null;
            }
            else
            {
                if (eventView.Location != null && eventView.Location.CityId != null)
                {
                    var resultVerification = await locationRepository.VerificationAddress(mapper.Map<Location>(eventView.Location));
                    switch (resultVerification)
                    {
                        case (VerificationAddressResult.NonExistentCity):
                            ModelState.AddModelError("inputLocationAddress", "Выбранного города нет в базе данных");
                            break;
                        case (VerificationAddressResult.NonExistentLocation):
                            ModelState.AddModelError("inputLocationAddress", "Данная локация уже существует, закройте модуль создания и выберите её в списке выше");
                            break;
                        case (VerificationAddressResult.LocationOutsideCity):
                            ModelState.AddModelError("inputLocationAddress", "Данная локация локация находится за пределами выбранного города! 30 км");
                            break;
                    }
                    if (eventView.Location.imagesLocation != null &&
                    !eventView.Location.imagesLocation.All(img => allowedExtensions.Contains(Path.GetExtension(img.FileName).ToLower())))
                    {
                        ModelState.AddModelError("Location.imagesLocation", $"Неподдерживаемый формат изображения");
                    }
                }
            }
        }
    }
   
    }

