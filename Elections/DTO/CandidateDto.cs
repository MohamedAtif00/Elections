namespace Elections.DTO
{
    public class CandidateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public IFormFile PersonalImage { get; set; }
        public IFormFile NationalIdImage { get; set; }
    }
}
