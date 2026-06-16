using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Сonstants;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoWeb.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserEventService userEvent;
        private readonly IMapper mapper;
        public UserController(IUserEventService userEvent, IMapper mapper)
        {
            this.userEvent = userEvent;
            this.mapper = mapper;
        }

     
     [HttpGet("event/[action]/{idEvent}")]
     public async Task<IActionResult> Join(int idEvent)
        {
            var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUser != null)
            {
                var result = await userEvent.JoinAsync(idUser, idEvent);
                switch (result)
                {
                    case JoinResult.SuccessNewRegistration:
                        return Ok(new { message = "Вы успешно присоединились к событию" });
                    case JoinResult.SuccessStatusUpdated:
                        return Ok(new { message = "Вы успешно присоединились к событию" });
                    case JoinResult.SuccessInReserve:
                        return Ok(new { message = "Вы в резерве" });
                    case JoinResult.AlreadyRegistered:
                        return Ok(new { message = "Ошибка, вы уже записаны на данное событие!" });
                    case JoinResult.UserNotFound:
                        return Ok(new { message = "Ошибка, такого пользователя нет в системе!" });
                    case JoinResult.EventNotFound:
                        return NotFound(new { message = "Ошибка, такого события нет в системе!" });
                    case JoinResult.NoAccessToEvent:
                        return Ok(new { message = "Ошибка, регистрация невозможна, событие недоступно!" });
                    case JoinResult.TimeCoincidences:
                        return Ok(new { message = "Ошибка, данное событие пересекается по времени с другим событием в которое вы записаны!" });
                }

            }

            return BadRequest(new { message = "Не удалось определить пользователя или сессия недействительна." });

        }

        [HttpGet("event/[action]/{idEvent}")]
        public async Task<IActionResult> Leave(int idEvent)
        {
            var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUser != null)
            {
                var result = await userEvent.LeaveUserAsync(idUser, idEvent);
                switch (result)
                {
                    case LeaveResult.AlreadyLeave:
                        return Ok(new { message = "Выписка невозможна вы уже выписаны" });
                    case LeaveResult.SuccessLeave:
                        return Ok(new { message = "Вы успешно выписались из события" });
                    case LeaveResult.EventNotFound:
                        return NotFound(new { message = "Мероприяте с таким id не найдено!" });
                    case LeaveResult.UserNotFound:
                        return NotFound(new { message = "Ошибка, такого пользователя нет в системе!" });
                    case LeaveResult.UserIsNotRegistered:
                        return Ok(new { message = "Вы не были зарегистриованны на данное событие!" });
                    case LeaveResult.EventIsOver:
                        return Ok(new { message = "Событие уже завершилось, выписка невозможна!" });
                    case LeaveResult.EvenWillStartSoon:
                        return Ok(new { message = "Событие скоро начнется, выписка невозможна!" });
                }
            }
            return BadRequest(new { message = "Не удалось определить пользователя или сессия недействительна." });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> MyEvents()
        {
            var listEvent = await userEvent.GetAllAttendedEventsUserAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var eventView = mapper.Map<List<EventSummaryViewModel>>(listEvent);
            return Ok(eventView);
        }
    }
}
