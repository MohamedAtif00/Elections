using Elections.Data;
using Elections.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Elections.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotersController : ControllerBase
    {
        private readonly VotingContext _context;

        public VotersController(VotingContext context)
        {
            _context = context;
        }

        [HttpPost("vote")]
        public async Task<IActionResult> Vote(int candidateId)
        {
            var voterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var voter = await _context.Voters.FindAsync(voterId);
            if (voter == null)
            {
                return NotFound("Voter not found.");
            }

            if (voter.HasVoted)
            {
                return BadRequest("Voter has already voted.");
            }

            var candidate = await _context.Candidates.FindAsync(candidateId);
            if (candidate == null)
            {
                return NotFound("Candidate not found.");
            }

            voter.HasVoted = true;
            candidate.Votes += 1;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }

}
