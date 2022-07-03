using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AgentApi.Model;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AgentApi.Services
{
    public class PublishJobOfferService
    {
        private readonly HttpClient _client;
        private readonly UserManager<User> _userManager;

        public PublishJobOfferService(IHttpClientFactory factory, UserManager<User> userManager)
        {
            _userManager = userManager;
            _client = factory.CreateClient();
            var uri = Environment.GetEnvironmentVariable("DISLINKT_BASE_URL") ?? "https://localhost:44322/";
            _client.BaseAddress = new Uri(uri);
        }

        public async Task<string> Publish(Guid userId, Guid jobOfferId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var jobOffer = ValidateUser(user, jobOfferId);

            var request = new
            {
                LinkToJobOffer = "http://localhost:3001/jobs/" + jobOffer.Id,
                Description = jobOffer.Description,
                JobTitle = jobOffer.JobTitle,
                Prerequisites = jobOffer.Prerequisites
            };
            
            _client.DefaultRequestHeaders.Add("X-API-KEY", user.Company.ApiKey);
            
            var response = await _client.PostAsync("/api/JobOffer", 
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            var entity = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(entity);

            jobOffer.IsPublished = true;

            await _userManager.UpdateAsync(user);
            
            return entity;
        }
        
        private JobOffer ValidateUser(User user, Guid jobOfferId)
        {
            if (user.Company == null)
                throw new Exception("User does not have a company");

            if (!user.Company.IsVerified)
                throw new Exception("User's company is not verified by an admin");

            if (string.IsNullOrEmpty(user.Company.ApiKey))
                throw new Exception("User's company is not published on Dislinkt");

            var jobOffer = user.Company.JobOffers.FirstOrDefault(x => x.Id == jobOfferId);
            if (jobOffer == null)
                throw new Exception("Job offer not found");

            if (jobOffer.IsPublished)
                throw new Exception("Job offer already published");

            return jobOffer;
        }
    }
}