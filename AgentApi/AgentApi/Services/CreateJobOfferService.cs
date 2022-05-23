using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentApi.Dtos;
using AgentApi.Model;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AgentApi.Services
{
    public class CreateJobOfferService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public CreateJobOfferService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<JobOffer> Create(string userId, NewJobOfferDto newJobOffer)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (!user.Company.IsVerified)
                throw new Exception("Company is still not verified");

            var mappedOffer = _mapper.Map<JobOffer>(newJobOffer);

            mappedOffer.Id = Guid.NewGuid();

            if (user.Company.JobOffers == null)
            {
                user.Company.JobOffers = new List<JobOffer>();
            }
            
            user.Company.JobOffers.Add(mappedOffer);
            await _userManager.UpdateAsync(user);

            return mappedOffer;
        }
    }
}