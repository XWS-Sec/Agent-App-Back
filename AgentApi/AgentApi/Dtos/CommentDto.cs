using System;
using System.ComponentModel.DataAnnotations;

namespace AgentApi.Dtos
{
    public class CommentDto
    {
        [Required(ErrorMessage = "Text is required")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Job offer is required")]
        public Guid JobOfferId { get; set; }
    }
}