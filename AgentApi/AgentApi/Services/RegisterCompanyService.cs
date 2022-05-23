using System;
using System.Threading.Tasks;
using AgentApi.Dtos;
using AgentApi.Model;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AgentApi.Services
{
    public class RegisterCompanyService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public RegisterCompanyService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Company> RegisterCompany(string userId, RegisterCompanyDto newCompany)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user.Company != null)
                throw new Exception("User already has a company registered or awaiting confirmation");

            var mappedCompany = _mapper.Map<Company>(newCompany);
            mappedCompany.Id = Guid.NewGuid();

            user.Company = mappedCompany;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception("Updating failed");

            return mappedCompany;
        }
    }
}