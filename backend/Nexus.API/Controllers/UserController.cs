using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Nexus.API.Data;
using Nexus.API.Models;
using Nexus.API.DTOs;

namespace Nexus.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpGet("{id}/words")]
        public async Task<ActionResult<IEnumerable<WordDto>>> GetUserWords(int id)
        {
            // Check that user exists
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Get the user's UserWords
            var userWords = await _context.UserWords
                .Where(uw => uw.UserId == id)
                .Include(uw => uw.Word)
                .ToListAsync();
            var words = userWords.Select(uw => uw.Word).ToList();
            var wordsDto = _mapper.Map<IEnumerable<WordDto>>(words);
            return Ok(wordsDto);

        }

        [HttpGet("{userId}/words/{wordId}")]
        public async Task<ActionResult<WordDto>> GetUserWord(int userId, int wordId)
        {
            // check that user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var userWord = await _context.UserWords
                .Where(uw => uw.UserId == userId && uw.WordId == wordId)
                .Include(uw => uw.Word)
                .FirstOrDefaultAsync();
            if (userWord == null)
            {
                return NotFound();
            }
            var wordDto = _mapper.Map<WordDto>(userWord.Word);
            return Ok(wordDto);
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }

        [HttpPost("{userId}/words")]
        public async Task<IActionResult> AddWordToUser(int userId, AddWordToUserDto addWordToUserDto)
        {
            // Check that user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Check that word exists
            var word = await _context.Words.FindAsync(addWordToUserDto.WordId);
            if (word == null)
            {
                return NotFound();
            }

            // Prevent duplicate links between the same user and word
            var userWordExists = await _context.UserWords
                .AnyAsync(uw => uw.UserId == userId && uw.WordId == addWordToUserDto.WordId);

            if (userWordExists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Word already assigned",
                    Detail = "This word is already in the user's list.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            // Create new UserWord
            var userWord = new UserWord(userId, addWordToUserDto.WordId);
            _context.UserWords.Add(userWord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
