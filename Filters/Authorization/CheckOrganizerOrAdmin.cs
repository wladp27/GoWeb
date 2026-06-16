using GoWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GoWeb.Filters.Authorization
{
    public class OrginizerOrAdminRequirement : IAuthorizationRequirement
    {
    }

    public class CheckAdminHandler : AuthorizationHandler<OrginizerOrAdminRequirement, EventSummaryViewModel>

    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrginizerOrAdminRequirement requirement, EventSummaryViewModel resource)
        {
            if (context.User.IsInRole("Администратор"))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class CheckOrganizerHandler : AuthorizationHandler<OrginizerOrAdminRequirement, EventSummaryViewModel>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrginizerOrAdminRequirement requirement, EventSummaryViewModel resource)
        {
            if (context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == resource.OrganizerId)
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

}
