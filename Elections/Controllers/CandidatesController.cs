using Elections.Data;
using Elections.DTO;
using Elections.Helper;
using Elections.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Sockets;

namespace Elections.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly VotingContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CandidatesController(VotingContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
        {
            return await _context.Candidates.ToListAsync();
        }

        [Authorize(Roles = "Candidate")]
        [HttpPost]
        public async Task<ActionResult<Candidate>> AddCandidate(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
            return Ok(candidate);
        }

        [HttpGet("subcategories")]
        public async Task<ActionResult<IEnumerable<CandidateDto>>> GetCandidatesBySubcategories([FromQuery] List<int> subCategoryIds)
        {
            if (subCategoryIds.Count() == 0)
            {
                var allCandidates = await _context.Candidates
                    .Select(candidate => new CandidateDto
                    {
                        Id = candidate.Id,
                        Name = candidate.Name,
                        Email = candidate.Email,
                        Gender = candidate.Gender,
                        PersonalImage = null,
                        NationalIdImage = null
                    })
                    .ToListAsync();

                return Ok(allCandidates);
            }
            else if (!subCategoryIds.Any())
            {
                return BadRequest("Subcategory IDs are required.");
            }

            var candidates = await _context.CandidateSubCategories
                .Where(cs => subCategoryIds.Contains(cs.SubCategoryId))
                .Select(cs => cs.Candidate)
                .Distinct()
                .Select(candidate => new CandidateDto
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    Email = candidate.Email,
                    Gender = candidate.Gender,
                    PersonalImage = null,
                    NationalIdImage = null
                })
                .ToListAsync();

            return Ok(candidates);
        }




        [HttpGet("images/{candidateId}/{imageType}")]
        public async Task<IActionResult> DownloadCandidateImage(int candidateId, string imageType)
        {
            var candidate = await _context.Candidates.FindAsync(candidateId);

            if (candidate == null)
            {
                return NotFound("Candidate not found.");
            }

            string imagePath = imageType == "personal" ? candidate.PersonalImagePath : candidate.NationalIdImagePath;

            if (string.IsNullOrEmpty(imagePath))
            {
                return NotFound("Image not found.");
            }

            string absolutePath;
            try
            {
                var env = _webHostEnvironment.WebRootPath;
                absolutePath = ImageHelper.GetImageFilePath(imagePath, env);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Image not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            try
            {
                var real = absolutePath.Replace("\\\\","/");
                if (System.IO.File.Exists(real))
                {
                    // Read the file into a byte array
                    byte[] imageData = System.IO.File.ReadAllBytes(absolutePath);


                    // Return the image data along with appropriate content type
                    return File(imageData, "image/jpeg");
                }
                else
                {
                    // If the file does not exist, return a 404 Not Found status
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("{candidateId}/add-subcategory/political-parties/{subCategoryId}")]
        public async Task<ActionResult> AddPoliticalPartySubcategory(int candidateId, int subCategoryId)
        {
            return await AddSubcategoryToCandidate(candidateId, subCategoryId);
        }

        //[HttpPost("{candidateId}/add-subcategory/independent-candidates/{subCategoryId}")]
        //public async Task<ActionResult> AddIndependentCandidateSubcategory(int candidateId, int subCategoryId)
        //{
        //    return await AddSubcategoryToCandidate(candidateId, subCategoryId);
        //}

        //[HttpPost("{candidateId}/add-subcategory/regional-parties/{subCategoryId}")]
        //public async Task<ActionResult> AddRegionalPartySubcategory(int candidateId, int subCategoryId)
        //{
        //    return await AddSubcategoryToCandidate(candidateId, subCategoryId);
        //}

        [HttpPost("{candidateId}/add-subcategory/issue-based-groups/{subCategoryId}")]
        public async Task<ActionResult> AddIssueBasedGroupSubcategory(int candidateId, int subCategoryId)
        {
            return await AddSubcategoryToCandidate(candidateId, subCategoryId);
        }

        //[HttpPost("{candidateId}/add-subcategory/ethnic-or-cultural-groups/{subCategoryId}")]
        //public async Task<ActionResult> AddEthnicOrCulturalGroupSubcategory(int candidateId, int subCategoryId)
        //{
        //    return await AddSubcategoryToCandidate(candidateId, subCategoryId);
        //}

        //[HttpPost("{candidateId}/add-subcategory/religious-groups/{subCategoryId}")]
        //public async Task<ActionResult> AddReligiousGroupSubcategory(int candidateId, int subCategoryId)
        //{
        //    return await AddSubcategoryToCandidate(candidateId, subCategoryId);
        //}

        //[HttpPost("{candidateId}/add-subcategory/youth-and-student-groups/{subCategoryId}")]
        //public async Task<ActionResult> AddYouthAndStudentGroupSubcategory(int candidateId, int subCategoryId)
        //{
        //    return await AddSubcategoryToCandidate(candidateId, subCategoryId);
        //}

        private async Task<ActionResult> AddSubcategoryToCandidate(int candidateId, int subCategoryId)
        {
            var candidate = await _context.Candidates.FindAsync(candidateId);
            if (candidate == null)
            {
                return NotFound("Candidate not found.");
            }

            var subcategory = await _context.SubCategories.FindAsync(subCategoryId);
            if (subcategory == null)
            {
                return NotFound("Subcategory not found.");
            }

            var existingEntry = await _context.CandidateSubCategories.FindAsync(candidateId, subCategoryId);
            if (existingEntry != null)
            {
                return BadRequest("Subcategory already assigned to the candidate.");
            }

            _context.CandidateSubCategories.Add(new CandidateSubCategory
            {
                CandidateId = candidateId,
                SubCategoryId = subCategoryId
            });

            await _context.SaveChangesAsync();

            return Ok("Subcategory added to the candidate successfully.");
        }
    }
}


