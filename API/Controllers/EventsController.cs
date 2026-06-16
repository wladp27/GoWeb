using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWeb.Repositories;
using GoWeb.Service;
using GoWeb.Сonstants;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace GoWeb.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService eventService;
       

        private readonly IUserEventService userEventService;
        public EventsController(IEventService eventService, IUserEventService userEventService, IUserEvent userEvent)
        {
            this.eventService = eventService;
            this.userEventService = userEventService;
           
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<ActionResult<EventIndexViewModel>> filter(int? selectedCity, int? selectedTypeEvent)
        {
            
             return await eventService.GetFilterEvents(selectedCity, selectedTypeEvent);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<EventUsersViewModel>> Event(int id)
        {
            var ev = await userEventService.GetEventsWithUserAsync(id);
            if (ev != null)
            {
                return ev;
            }
            return NotFound(); 
        }


        
       

    }
}
