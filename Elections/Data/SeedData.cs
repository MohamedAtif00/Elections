using Elections.Model;
using Microsoft.EntityFrameworkCore;

namespace Elections.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new VotingContext(serviceProvider.GetRequiredService<DbContextOptions<VotingContext>>()))
            {
                if (context.Categories.Any())
                {
                    return;   // DB has been seeded
                }

                var categories = new Category[]
                {
                    new Category { Name = "Political Parties", SubCategories = new SubCategory[]
                    {
                        new SubCategory { Name = "Liberal Party" },
                        new SubCategory { Name = "Conservative Party" },
                        new SubCategory { Name = "Socialist Party" },
                        new SubCategory { Name = "Green Party" },
                        new SubCategory { Name = "Libertarian Party" },
                        new SubCategory { Name = "Nationalist Party" },
                        new SubCategory { Name = "Centrist Party" }
                    }},
                    new Category { Name = "Independent Candidates", SubCategories = new SubCategory[]
                    {
                        new SubCategory { Name = "Individuals who are not affiliated with any political party" }
                    }},
                    new Category { Name = "Regional Parties", SubCategories = new SubCategory[]
                    {
                        new SubCategory { Name = "Parties that operate in specific regions or states and focus on regional issues" }
                    }},
                    new Category { Name = "Issue-Based Groups", SubCategories = new SubCategory[]
                    {
                        new SubCategory { Name = "Environmental Advocacy" },
                        new SubCategory { Name = "Human Rights" },
                        new SubCategory { Name = "Economic Reform" },
                        new SubCategory { Name = "Social Justice" },
                        new SubCategory { Name = "Healthcare Reform" }
                    }},
                    new Category { Name = "Ethnic or Cultural Groups", SubCategories = new SubCategory[]
                    {
                        new SubCategory { Name = "Groups representing specific ethnic or cultural communities" }
                    }},
                    new Category { Name = "Religious Groups", SubCategories = new SubCategory[]
                    {
                        new SubCategory { Name = "Political groups that represent religious communities or are based on religious ideologies" }
                    }},
                    new Category { Name = "Youth and Student Groups", SubCategories = new SubCategory[]
                    {
                        new SubCategory { Name = "Organizations focused on the interests of younger voters and students" }
                    }}
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
        }
    }
}
