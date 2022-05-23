using System;
using System.Threading.Tasks;
using AgentApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualBasic;

namespace AgentApi.Attributes
{
    public class AdminSeedAttribute : IAsyncActionFilter
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AdminSeedAttribute(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var admin = await _userManager.FindByNameAsync("admin");
            if (admin == null)
            {
                admin = new User()
                {
                    UserName = "admin",
                    Email = "admin@xws-sec.com",
                    Name = "admin",
                    Surname = "admin"
                };

                var password = Environment.GetEnvironmentVariable("XWS_PKI_ADMINPASS");

                await _userManager.CreateAsync(admin, password);

                await _roleManager.CreateAsync(new Role("admin"));
                await _userManager.AddToRoleAsync(admin, "admin");
            }
            
            await next();
        }
    }
}