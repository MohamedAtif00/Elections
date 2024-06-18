using System.ComponentModel.DataAnnotations;

namespace Elections.Model
{
    public class Candidate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Gender { get; set; }
        public string PersonalImagePath { get; set; }
        public string NationalIdImagePath { get; set; }
        public int Votes { get; set; } = 0;
        public ICollection<CandidateSubCategory> CandidateSubCategories { get; set; }  // Navigation property for chosen subcategories
    }
}
