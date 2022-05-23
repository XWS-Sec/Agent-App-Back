using System;
using AspNetCore.Identity.MongoDbCore.Models;

namespace AgentApi.Model
{
    public class User : MongoIdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public Company Company { get; set; }
    }
}