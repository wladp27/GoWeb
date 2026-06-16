using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Сonstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoWeb.Areas.User.Controllers
{
    [Area("User")]
    public class EventController : Controller
    {
        private readonly IUserEventService userEvent;
        private readonly IMapper mapper;

        public EventController(IUserEventService userEvent, IMapper mapper)
        {
            this.mapper = mapper;
            this.userEvent = userEvent;
        }

        [Authorize]
        public async Task<IActionResult> Join(int idEvent, string? returnUrl)
        {
            var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(idUser!=null)
            {
                var result=await userEvent.JoinAsync(idUser, idEvent);
                switch(result)
                {
                    case JoinResult.SuccessNewRegistration:
                        TempData["SuccessMessage"] = "Вы успешно присоединились к событию";
                        break;
                    case JoinResult.SuccessStatusUpdated:
                        TempData["SuccessMessage"] = "Вы успешно присоединились к событию";
                        break;
                    case JoinResult.SuccessInReserve:
                        TempData["SuccessMessage"] = "Вы в резерве";
                        break;
                    case JoinResult.AlreadyRegistered:
                        TempData["ErrorMessage"] = "Ошибка, вы уже записаны на данное событие!";
                        break;
                    case JoinResult.UserNotFound:
                        TempData["ErrorMessage"] = "Ошибка, такого пользователя нет в системе!";
                        break;
                    case JoinResult.EventNotFound:
                        TempData["ErrorMessage"] = "Ошибка, такого события нет в системе!";
                        break;
                    case JoinResult.NoAccessToEvent:
                        TempData["ErrorMessage"] = "Ошибка, регистрация невозможна, событие недоступно!";
                        break;
                    case JoinResult.TimeCoincidences:
                        TempData["ErrorMessage"] = "Ошибка, данное событие пересекается по времени с другим событием в которое вы записаны!";
                        break;
                }
                
            }
    
                return RedirectToAction("Event", "Event", new { area = "",id=idEvent, returnUrl = returnUrl });
        }

        [Authorize]
        public async Task<IActionResult> Leave(int idEvent, string? returnUrl)
        {
            var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUser != null)
            {
                var result = await userEvent.LeaveUserAsync(idUser, idEvent);
                switch (result)
                {
                    case LeaveResult.AlreadyLeave:
                        TempData["ErrorMessage"] = "Выписка невозможна вы уже выписаны";
                        break;
                    case LeaveResult.SuccessLeave:
                        TempData["SuccessMessage"] = "Вы успешно выписались из события";
                        break;
                    case LeaveResult.EventNotFound:
                        TempData["ErrorMessage"] = "Мероприяте с таким id не найдено!";
                        break;
                    case LeaveResult.UserNotFound:
                        TempData["ErrorMessage"] = "Ошибка, такого пользователя нет в системе!";
                        break;
                    case LeaveResult.UserIsNotRegistered:
                        TempData["ErrorMessage"] = "Вы не были зарегистриованны на данное событие!";
                        break;
                    case LeaveResult.EventIsOver:
                        TempData["ErrorMessage"] = "Событие уже завершилось, выписка невозможна!";
                        break;
                    case LeaveResult.EvenWillStartSoon:
                        TempData["ErrorMessage"] = "Событие скоро начнется, выписка невозможна!";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("MyEvents", "Event", new { area = "User" });
        }

        [Authorize]
        public async Task<IActionResult> MyEvents()
        {
            var listEvent = await  userEvent.GetAllAttendedEventsUserAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var eventView = mapper.Map<List<EventSummaryViewModel>>(listEvent);
            return View(eventView);
        }

    }
}
