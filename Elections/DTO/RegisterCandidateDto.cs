using System.ComponentModel.DataAnnotations;

namespace Elections.DTO
{
    public class RegisterCandidateDto
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
        public string Gender { get; set; }

        [Required]
        public IFormFile PersonalImage { get; set; }

        [Required]
        public IFormFile NationalIdImage { get; set; }

    }
}
