using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AgentApi.Contracts;
using AgentApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace AgentApi.Services
{
    public class PublishCompanyService
    {
        private readonly UserManager<User> _userManager;
        private readonly HttpClient _httpClient;

        public PublishCompanyService(UserManager<User> userManager, IHttpClientFactory factory)
        {
            _userManager = userManager;
            _httpClient = factory.CreateClient();
            var uri = Environment.GetEnvironmentVariable("DISLINKT_BASE_URL") ?? "https://localhost:44322/";
            _httpClient.BaseAddress = new Uri(uri);
        }

        public async Task<string> Publish(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            ValidateUser(user);

            var request = new
            {
                LinkToCompany = "https://localhost:44323/api/Company/" + user.Company.Id,
                Name = user.Company.Name,
                Email = user.Company.Email,
                PhoneNumber = user.Company.PhoneNumber
            };

            var response = await _httpClient.PostAsync("/api/Company", 
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!responseContent.Contains("{")) return responseContent;
            var entity = JsonConvert.DeserializeObject<PublishCompanyContract>(responseContent);
            
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error message : " + responseContent);

            user.Company.ApiKey = entity.Message;

            await _userManager.UpdateAsync(user);
            
            return entity.Message;
        }

        private void ValidateUser(User user)
        {
            if (user.Company == null)
                throw new Exception("User does not have a company");

            if (!user.Company.IsVerified)
                throw new Exception("User's company is not verified by an admin");

            if (!string.IsNullOrEmpty(user.Company.ApiKey))
                throw new Exception("User's company is already published on Dislinkt");
        }
    }
}