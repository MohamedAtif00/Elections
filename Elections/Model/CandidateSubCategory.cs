namespace Elections.Model
{
    public class CandidateSubCategory
    {
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
    }
}
