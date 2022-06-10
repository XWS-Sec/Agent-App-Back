using AgentApi.Dtos;
using AgentApi.Model;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentApi.Services
{
    public class UpdateCompanyService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UpdateCompanyService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Company> UpdateCompany(string userId,UpdateCompanyDto updateCompanyDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user.Company == null)
                throw new Exception("User doesn't have company");

            var newCompany = _mapper.Map<Company>(updateCompanyDto);
            SetCompany(newCompany, user.Company);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception("Updating failed");

            return user.Company;
        }


        private static void SetCompany(Company newCompany,Company updatedCompany)
        {
            updatedCompany.Address = string.IsNullOrEmpty(newCompany.Address) ? updatedCompany.Address : newCompany.Address;
            updatedCompany.Name = string.IsNullOrEmpty(newCompany.Name) ? updatedCompany.Name : newCompany.Name;
            updatedCompany.Email = string.IsNullOrEmpty(newCompany.Email) ? updatedCompany.Email : newCompany.Email;
            updatedCompany.PhoneNumber = string.IsNullOrEmpty(newCompany.PhoneNumber) ? updatedCompany.PhoneNumber : newCompany.PhoneNumber;
            updatedCompany.Description = string.IsNullOrEmpty(newCompany.Description) ? updatedCompany.Description : newCompany.Description;
        }
    }
}
