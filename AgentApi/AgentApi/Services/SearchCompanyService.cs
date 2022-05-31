using System;
using System.Threading.Tasks;
using AgentApi.Model;
using MongoDB.Driver;

namespace AgentApi.Services
{
    public class SearchCompanyService
    {
        private readonly IMongoCollection<User> _userCollection;

        public SearchCompanyService(IMongoClient client)
        {
            _userCollection = client.GetDatabase("AgentApi").GetCollection<User>("users");
        }

        public async Task<Company> GetById(Guid id)
        {
            var userCursor = await _userCollection.FindAsync(x => x.Company != null && x.Company.Id == id);

            var user = await userCursor.FirstOrDefaultAsync();

            if (user == null)
                return null;

            return user.Company;
        }
    }
}