using Elections.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Elections.Data
{
    public class VotingContext : DbContext
    {
        public VotingContext(DbContextOptions<VotingContext> options) : base(options)
        {
        }

        public DbSet<Voter> Voters { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<CandidateSubCategory> CandidateSubCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasMany(c => c.SubCategories).WithOne(sc => sc.Category).HasForeignKey(sc => sc.CategoryId);

            modelBuilder.Entity<CandidateSubCategory>()
               .HasKey(cs => new { cs.CandidateId, cs.SubCategoryId });

            modelBuilder.Entity<CandidateSubCategory>()
                .HasOne(cs => cs.Candidate)
                .WithMany(c => c.CandidateSubCategories)
                .HasForeignKey(cs => cs.CandidateId);

            modelBuilder.Entity<CandidateSubCategory>()
                .HasOne(cs => cs.SubCategory)
                .WithMany(sc => sc.CandidateSubCategories)
                .HasForeignKey(cs => cs.SubCategoryId);
        }
    }
}
