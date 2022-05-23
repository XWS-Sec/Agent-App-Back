using System;

namespace AgentApi.Model
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}