using System.ComponentModel.DataAnnotations;

namespace AgentApi.Dtos
{
    public class RegisterCompanyDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }
}