using System;
using System.Threading.Tasks;
using AgentApi.Model;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace AgentApi.Services
{
    public class VerifyCompanyService
    {
        private readonly UserManager<User> _userManager;
        private IMongoCollection<User> _userCollection;
        private readonly RoleManager<Role> _roleManager;

        public VerifyCompanyService(UserManager<User> userManager, IMongoClient client, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userCollection = client.GetDatabase("AgentApi").GetCollection<User>("users");
        }

        public async Task Verify(Guid companyId)
        {
            var filter = Builders<User>.Filter.Where(x => x.Company != null && x.Company.Id == companyId);

            var user = await _userCollection.FindAsync(filter);
            var foundUser = await user.FirstOrDefaultAsync();

            if (foundUser == null)
                throw new Exception("User with that company id not found.");

            if (foundUser.Company.IsVerified)
                throw new Exception("That company is already verified");

            foundUser.Company.IsVerified = true;

            await _userManager.UpdateAsync(foundUser);
            await _userManager.RemoveFromRoleAsync(foundUser, "StandardUser");

            await _roleManager.CreateAsync(new Role("CompanyOwner"));
            await _userManager.AddToRoleAsync(foundUser, "CompanyOwner");
        }
    }
}