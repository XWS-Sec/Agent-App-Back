using System.Threading.Tasks;
using AgentApi.Dtos;
using AgentApi.Model;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgentApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;

        public RegistrationController(UserManager<User> userManager, IMapper mapper, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post(RegisterUserDto newUser)
        {
            var mappedUser = _mapper.Map<User>(newUser);

            var result = await _userManager.CreateAsync(mappedUser, newUser.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _roleManager.CreateAsync(new Role("StandardUser"));
            await _userManager.AddToRoleAsync(mappedUser, "StandardUser");
            
            return Ok(mappedUser);
        }
    }
}