using System;
using System.Threading.Tasks;
using AgentApi.Attributes;
using AgentApi.Dtos;
using AgentApi.Model;
using AgentApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace AgentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobOfferController : ControllerBase
    {
        private readonly CreateJobOfferService _createJobOfferService;
        private readonly CreateCommentOnJobOfferService _createCommentOnJobOfferService;
        private readonly UserManager<User> _userManager;

        public JobOfferController(CreateJobOfferService createJobOfferService, UserManager<User> userManager, CreateCommentOnJobOfferService createCommentOnJobOfferService)
        {
            _createJobOfferService = createJobOfferService;
            _userManager = userManager;
            _createCommentOnJobOfferService = createCommentOnJobOfferService;
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

        [HttpPost("comment")]
        [TypeFilter(typeof(CustomAuthorizeAttribute), Arguments = new object[] { "StandardUser" })]
        public async Task<IActionResult> PostComment(CommentDto newComment)
        {
            var userId = _userManager.GetUserId(User);

            try
            {
                var comment = await _createCommentOnJobOfferService.Create(userId, newComment);

                return Ok(comment);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}