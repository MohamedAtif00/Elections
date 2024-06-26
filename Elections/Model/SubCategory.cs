﻿namespace Elections.Model
{
    public class SubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<CandidateSubCategory> CandidateSubCategories { get; set; }  // Navigation property for candidates
    }
}
