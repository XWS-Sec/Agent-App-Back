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
    [Route("/api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly RegisterCompanyService _registerCompanyService;
        private readonly VerifyCompanyService _verifyCompanyService;
        private readonly UserManager<User> _userManager;

        public CompanyController(RegisterCompanyService registerCompanyService, UserManager<User> userManager, VerifyCompanyService verifyCompanyService)
        {
            _registerCompanyService = registerCompanyService;
            _userManager = userManager;
            _verifyCompanyService = verifyCompanyService;
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

        [HttpPut("{companyId}")]
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

    }
}