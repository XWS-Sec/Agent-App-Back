using System;
using System.Collections.Generic;

namespace AgentApi.Model
{
    public class JobOffer
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string JobTitle { get; set; }
        public string Prerequisites { get; set; }
        public bool IsPublished { get; set; }

        public IList<Comment> Comments { get; set; }
    }
}