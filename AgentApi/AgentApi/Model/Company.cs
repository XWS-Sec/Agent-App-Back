using System;
using System.Collections.Generic;

namespace AgentApi.Model
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string ApiKey { get; set; }

        public bool IsVerified { get; set; }

        public IList<Comment> Comments { get; set; }
        public IList<JobOffer> JobOffers { get; set; }  
    }
}