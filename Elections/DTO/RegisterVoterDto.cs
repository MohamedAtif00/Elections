using System.ComponentModel.DataAnnotations;

namespace Elections.DTO
{
    public class RegisterVoterDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public IFormFile NationalIdImage { get; set; }  // Add NationalIdImage property
    }
}

