using Elections.Data;
using Elections.DTO;
using Elections.Helper;
using Elections.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elections.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly VotingContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthController(VotingContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("register/voter")]
        public async Task<ActionResult<RegisterLoginResponseDto>> RegisterVoter([FromForm] RegisterVoterDto registerVoterDto)
        {
            if (await _context.Voters.AnyAsync(v => v.Email == registerVoterDto.Email))
            {
                return BadRequest("Email is already registered.");
            }

            PasswordHelper.CreatePasswordHash(registerVoterDto.Password, out string passwordHash, out string passwordSalt);

            var nationalIdImagePath = await ImageHelper.SaveImageAsync(registerVoterDto.NationalIdImage, _webHostEnvironment.WebRootPath, "uploads/voters");

            var voter = new Voter
            {
                Name = registerVoterDto.Name,
                Email = registerVoterDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                HasVoted = false,
                NationalIdImagePath = nationalIdImagePath
            };

            _context.Voters.Add(voter);
            await _context.SaveChangesAsync();

            var token = TokenHelper.GenerateJwtToken(voter.Id, voter.Email, "Voter", _configuration);

            return Ok(new RegisterLoginResponseDto
            {
                Token = token,
                Email = voter.Email,
                DisplayName = voter.Name,
                Role = "Voter",
                Id = voter.Id  // Return the ID
            });
        }

        [HttpPost("register/candidate")]
        public async Task<ActionResult<RegisterLoginResponseDto>> RegisterCandidate([FromForm] RegisterCandidateDto registerCandidateDto)
        {
            if (await _context.Candidates.AnyAsync(c => c.Email == registerCandidateDto.Email))
            {
                return BadRequest("Email is already registered.");
            }

            PasswordHelper.CreatePasswordHash(registerCandidateDto.Password, out string passwordHash, out string passwordSalt);

            var personalImagePath = await ImageHelper.SaveImageAsync(registerCandidateDto.PersonalImage, _webHostEnvironment.WebRootPath, "uploads/candidates");
            var nationalIdImagePath = await ImageHelper.SaveImageAsync(registerCandidateDto.NationalIdImage, _webHostEnvironment.WebRootPath, "uploads/candidates");

            var candidate = new Candidate
            {
                Name = registerCandidateDto.Name,
                Email = registerCandidateDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Gender = registerCandidateDto.Gender,
                PersonalImagePath = personalImagePath,
                NationalIdImagePath = nationalIdImagePath,
            };

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            var token = TokenHelper.GenerateJwtToken(candidate.Id, candidate.Email, "Candidate", _configuration);

            return Ok(new RegisterLoginResponseDto
            {
                Token = token,
                Email = candidate.Email,
                DisplayName = candidate.Name,
                Role = "Candidate",
                Id = candidate.Id  // Return the ID
            });
        }

        [HttpPost("login/voter")]
        public async Task<ActionResult<RegisterLoginResponseDto>> LoginVoter([FromBody] LoginDto loginDto)
        {
            var voter = await _context.Voters.FirstOrDefaultAsync(v => v.Email == loginDto.email);
            if (voter == null || !PasswordHelper.VerifyPasswordHash(loginDto.password, voter.PasswordHash, voter.PasswordSalt))
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = TokenHelper.GenerateJwtToken(voter.Id, voter.Email, "Voter", _configuration);

            return Ok(new RegisterLoginResponseDto
            {
                Token = token,
                Email = voter.Email,
                DisplayName = voter.Name,
                Role = "Voter",
                Id = voter.Id
            });
        }

        [HttpPost("login/candidate")]
        public async Task<ActionResult<RegisterLoginResponseDto>> LoginCandidate([FromBody] LoginDto loginDto)
        {
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Email == loginDto.email);
            if (candidate == null || !PasswordHelper.VerifyPasswordHash(loginDto.password, candidate.PasswordHash, candidate.PasswordSalt))
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = TokenHelper.GenerateJwtToken(candidate.Id, candidate.Email, "Candidate", _configuration);

            return Ok(new RegisterLoginResponseDto
            {
                Token = token,
                Email = candidate.Email,
                DisplayName = candidate.Name,
                Role = "Candidate",
                Id = candidate.Id

            });
        }
    }
}
