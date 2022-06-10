using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentApi.Attributes;
using AgentApi.Dtos;
using AgentApi.Model;
using AgentApi.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgentApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly RegisterCompanyService _registerCompanyService;
        private readonly UpdateCompanyService _updateCompanyService;
        private readonly VerifyCompanyService _verifyCompanyService;
        private readonly SearchCompanyService _searchCompanyService;
        private readonly PublishCompanyService _publishCompanyService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public CompanyController(RegisterCompanyService registerCompanyService, UserManager<User> userManager, VerifyCompanyService verifyCompanyService, SearchCompanyService searchCompanyService, IMapper mapper, PublishCompanyService publishCompanyService, UpdateCompanyService updateCompanyService)
        {
            _registerCompanyService = registerCompanyService;
            _userManager = userManager;
            _verifyCompanyService = verifyCompanyService;
            _searchCompanyService = searchCompanyService;
            _mapper = mapper;
            _publishCompanyService = publishCompanyService;
            _updateCompanyService = updateCompanyService;
        }

        [HttpGet("{companyId}")]
        public async Task<IActionResult> Get(Guid companyId)
        {
            var company = await _searchCompanyService.GetById(companyId);

            return company == null
                ? NotFound("Company with that id does not exist")
                : Ok(_mapper.Map<SearchCompanyDto>(company));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _searchCompanyService.GetAllVerified();
            if (companies == null)
                return NotFound("There is not existing companies!");

            return Ok(_mapper.Map<List<VerifiedCompanyDto>>(companies));
        }

        [HttpPost]
        [TypeFilter(typeof(CustomAuthorizeAttribute), Arguments = new object[]{"StandardUser"})]
        public async Task<IActionResult> Register(RegisterCompanyDto newCompany)
        {
            var userId = _userManager.GetUserId(User);

            try
            {
                var company = await _registerCompanyService.RegisterCompany(userId, newCompany);

                return Ok(company);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("verify/{companyId}")]
        [TypeFilter(typeof(CustomAuthorizeAttribute), Arguments = new object[] { "admin" })]
        public async Task<IActionResult> Verify(Guid companyId)
        {
            try
            {
                await _verifyCompanyService.Verify(companyId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpPut]
        [TypeFilter(typeof(CustomAuthorizeAttribute), Arguments = new object[] { "CompanyOwner" })]
        public async Task<IActionResult> Update(UpdateCompanyDto updateCompanyDto)
        {
            var userId = _userManager.GetUserId(User);
            Company response;
            try
            {
                response = await _updateCompanyService.UpdateCompany(userId,updateCompanyDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(response);
        }

        [HttpPost("publish")]
        [TypeFilter(typeof(CustomAuthorizeAttribute), Arguments = new object[] { "CompanyOwner" })]
        public async Task<IActionResult> PublishOnDislinkt()
        {
            var userId = Guid.Parse(_userManager.GetUserId(User));
            
            try
            {
                var receivedApiKey = await _publishCompanyService.Publish(userId);
                return Ok(receivedApiKey);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}