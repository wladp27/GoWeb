using GoWeb.Interfaces;
using GoWeb.Service;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GoWeb.Filters
{
    public class EnsureEventExists : IAsyncActionFilter
    {
        private readonly IEventService eventService;
        public EnsureEventExists(IEventService eventService) 
        {
            this.eventService = eventService;
        }


        public async Task  OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ev = await eventService.GetByIdAsync((int)context.ActionArguments["idEvent"]);
            if (ev == null)
            {
                if (context.Controller is Controller)
                {
                    var controller = context.Controller as Controller;
                    controller.TempData["ErrorMessage"] = "События с таким id не существует";
                }
                context.Result = new RedirectToActionResult(nameof(GoWeb.Areas.Manage.Controllers.EventController.Index),
                                        nameof(GoWeb.Areas.Manage.Controllers.EventController).Replace("Controller", ""), new { area = "" });
                return;
            }
            context.HttpContext.Items[typeof(Event)] = ev;
            await next();
        }
    }
}
