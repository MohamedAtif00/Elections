using Elections.Data;
using Elections.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elections.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly VotingContext _context;

        public CategoriesController(VotingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesWithSubcategories()
        {
            var categories = await _context.Categories
                .Include(c => c.SubCategories)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Subcategories = c.SubCategories.Select(s => new SubcategoryDto
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList()
                })
                .ToListAsync();

            return Ok(categories);
        }
    }
}

