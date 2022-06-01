using System;

namespace AgentApi.Contracts
{
    public class PublishCompanyContract
    {
        public Guid CorrelationId { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
    }
}