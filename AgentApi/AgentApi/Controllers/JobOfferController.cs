using System;
using System.Threading.Tasks;
using AgentApi.Attributes;
using AgentApi.Dtos;
using AgentApi.Model;
using AgentApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobOfferController : ControllerBase
    {
        private readonly CreateJobOfferService _createJobOfferService;
        private readonly UserManager<User> _userManager;

        public JobOfferController(CreateJobOfferService createJobOfferService, UserManager<User> userManager)
        {
            _createJobOfferService = createJobOfferService;
            _userManager = userManager;
        }

        [HttpPost]
        [TypeFilter(typeof(CustomAuthorizeAttribute), Arguments = new object[] { "CompanyOwner" })]
        public async Task<IActionResult> Create(NewJobOfferDto newJobOffer)
        {
            var userId = _userManager.GetUserId(User);
            try
            {
                var jobOffer = await _createJobOfferService.Create(userId, newJobOffer);

                return Ok(jobOffer);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}