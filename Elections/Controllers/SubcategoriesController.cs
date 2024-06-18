using Elections.Data;
using Elections.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elections.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoriesController : ControllerBase
    {
        private readonly VotingContext _context;

        public SubcategoriesController(VotingContext context)
        {
            _context = context;
        }

        [HttpGet("political-parties")]
        public ActionResult<IEnumerable<SubcategoryDto>> GetPoliticalPartiesSubcategories()
        {
            return GetSubcategoriesForCategory("Political Parties");
        }

        [HttpGet("independent-candidates")]
        public ActionResult<IEnumerable<SubcategoryDto>> GetIndependentCandidatesSubcategories()
        {
            return GetSubcategoriesForCategory("Independent Candidates");
        }

        [HttpGet("regional-parties")]
        public ActionResult<IEnumerable<SubcategoryDto>> GetRegionalPartiesSubcategories()
        {
            return GetSubcategoriesForCategory("Regional Parties");
        }

        [HttpGet("issue-based-groups")]
        public ActionResult<IEnumerable<SubcategoryDto>> GetIssueBasedGroupsSubcategories()
        {
            return GetSubcategoriesForCategory("Issue-Based Groups");
        }

        [HttpGet("ethnic-or-cultural-groups")]
        public ActionResult<IEnumerable<SubcategoryDto>> GetEthnicOrCulturalGroupsSubcategories()
        {
            return GetSubcategoriesForCategory("Ethnic or Cultural Groups");
        }

        [HttpGet("religious-groups")]
        public ActionResult<IEnumerable<SubcategoryDto>> GetReligiousGroupsSubcategories()
        {
            return GetSubcategoriesForCategory("Religious Groups");
        }

        [HttpGet("youth-and-student-groups")]
        public ActionResult<IEnumerable<SubcategoryDto>> GetYouthAndStudentGroupsSubcategories()
        {
            return GetSubcategoriesForCategory("Youth and Student Groups");
        }

        private ActionResult<IEnumerable<SubcategoryDto>> GetSubcategoriesForCategory(string categoryName)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var subcategories = _context.SubCategories
                .Where(s => s.CategoryId == category.Id)
                .Select(s => new SubcategoryDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToList();

            return Ok(subcategories);
        }
    }
}
