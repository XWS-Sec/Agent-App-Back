using System;
using System.Threading;
using System.Threading.Tasks;
using AgentApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace AgentApi.HostedServices
{
    public class AdminSeedHostedService : IHostedService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AdminSeedHostedService(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var admin = await _userManager.FindByNameAsync("admin");
            if (admin != null)
                return;

            admin = new User()
            {
                UserName = "admin",
                Name = "admin",
                Surname = "admin",
                Email = "admin@xws-sec.com",
                EmailConfirmed = true,
            };

            var password = Environment.GetEnvironmentVariable("XWS_PKI_ADMINPASS");

            await _userManager.CreateAsync(admin, password);

            await _roleManager.CreateAsync(new Role("admin"));
            await _userManager.AddToRoleAsync(admin, "admin");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}