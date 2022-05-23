using System.Threading.Tasks;
using AgentApi.Dtos;
using AgentApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgentApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto credentials)
        {
            var user = await _userManager.FindByNameAsync(credentials.Username);

            if (user == null)
                return BadRequest("User with that username does not exist");

            var result = await _signInManager.PasswordSignInAsync(user, credentials.Password, false, false);

            if (!result.Succeeded)
            {
                return BadRequest("Wrong password");
            }
            
            return Ok(user);
        }
    }
}