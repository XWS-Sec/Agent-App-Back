using System;
using AspNetCore.Identity.MongoDbCore.Models;

namespace AgentApi.Model
{
    public class Role : MongoIdentityRole<Guid>
    {
        public Role()
        {
        }

        public Role(string name) : base(name)
        {
        }
    }
}