using GoWeb.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GoWeb.Filters
{
    public class EnsureStatusEventExists : IAsyncActionFilter
    {
        private readonly IStatusEventService statusEventService;
        public EnsureStatusEventExists(IStatusEventService statusEventService)
        {
            this.statusEventService = statusEventService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("idStatusEvent", out var idValue) && idValue is int id)
            {
                var result = await statusEventService.GetByIdAsync(id); 
                if (result == null)
                {
                    if (context.Controller is Controller)
                    {
                        var controller = context.Controller as Controller;
                        controller.TempData["ErrorMessage"] = "Ошибка! Данного типа статуса не сущесвует";
                        context.Result = new RedirectToActionResult(nameof(GoWeb.Areas.Manage.Controllers.EventController.Index),
                                            nameof(GoWeb.Areas.Manage.Controllers.EventController).Replace("Controller", ""), new { area = "" });
                        return;
                    }
                }
                await next();
            }
        }
    }
}
