using System.Text;
using System.Threading.Tasks;
using AgentApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgentApi.Attributes
{
    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly UserManager<User> _userManager;
        private readonly string _roleToAuthorize;

        public CustomAuthorizeAttribute(UserManager<User> userManager, string roleToAuthorize = "")
        {
            _userManager = userManager;
            _roleToAuthorize = roleToAuthorize;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claims = context.HttpContext.User;
            var userId = _userManager.GetUserId(claims);

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!string.IsNullOrEmpty(_roleToAuthorize))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (!await _userManager.IsInRoleAsync(user, _roleToAuthorize))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
            
            await base.OnActionExecutionAsync(context, next);
        }
    }
}