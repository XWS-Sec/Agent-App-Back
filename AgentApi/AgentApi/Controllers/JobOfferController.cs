using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AgentApi.Attributes;
using AgentApi.Dtos;
using AgentApi.Model;
using AgentApi.Services;
using AutoMapper;
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
        private readonly PublishJobOfferService _publishJobOfferService;
        private readonly UserManager<User> _userManager;
        private readonly SearchOfferService _searchOfferService;

        public JobOfferController(CreateJobOfferService createJobOfferService, UserManager<User> userManager, CreateCommentOnJobOfferService createCommentOnJobOfferService, SearchOfferService searchOfferService, PublishJobOfferService publishJobOfferService)
        {
            _createJobOfferService = createJobOfferService;
            _userManager = userManager;
            _createCommentOnJobOfferService = createCommentOnJobOfferService;
            _searchOfferService = searchOfferService;
            _publishJobOfferService = publishJobOfferService;
        }

        [HttpGet]
        [TypeFilter(typeof(CustomAuthorizeAttribute), Arguments = new object[] { "StandardUser" })]
        public IActionResult Get(string company, string prerequisites, string title)
        {
            return Ok(_searchOfferService.FindJobOffers(new SearchOfferDto()
            {
                Company = company,
                Prerequisites = prerequisites,
                Title = title
            }));
        }

        [HttpGet("{jobOfferId}")]
        public IActionResult Get(Guid jobOfferId)
        {
            var jobOffer = _searchOfferService.GetById(jobOfferId);

            return jobOffer == null
                ? NotFound("Job offer with that id is not found")
                : Ok(jobOffer);
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

        [HttpPost("publish/{jobOfferId}")]
        [TypeFilter(typeof(CustomAuthorizeAttribute), Arguments = new object[] { "CompanyOwner" })]
        public async Task<IActionResult> Publish(Guid jobOfferId)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User));

            try
            {
                var response = await _publishJobOfferService.Publish(userId, jobOfferId);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}