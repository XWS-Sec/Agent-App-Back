using System;
using System.ComponentModel.DataAnnotations;

namespace AgentApi.Dtos
{
    public class NewJobOfferDto
    {
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Job title is required")]
        public string JobTitle { get; set; }
        [Required(ErrorMessage = "Prerequisites are required")]
        public string Prerequisites { get; set; }
    }
}