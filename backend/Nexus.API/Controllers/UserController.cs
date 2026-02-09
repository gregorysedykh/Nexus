using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Nexus.API.Data;
using Nexus.API.Models;
using Nexus.API.DTOs;
using Npgsql;

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

            var words = userWords
                .Select(uw => uw.Word)
                .GroupBy(w => new
                {
                    Term = NormaliseTerm(w.Term),
                    LanguageCode = NormaliseLanguageCode(w.LanguageCode)
                })
                .Select(group => group.OrderBy(w => w.Id).First())
                .ToList();

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

            var normalisedTerm = NormaliseTerm(word.Term);
            var normalisedLanguageCode = NormaliseLanguageCode(word.LanguageCode);

            // Resolve all equivalent word ids and pick one canonical id for storage.
            var matchingWordIds = await _context.Words
                .Where(w =>
                    w.Term.Trim().ToLower() == normalisedTerm &&
                    w.LanguageCode.Trim().ToLower() == normalisedLanguageCode)
                .OrderBy(w => w.Id)
                .Select(w => w.Id)
                .ToListAsync();

            if (matchingWordIds.Count == 0)
            {
                return NotFound();
            }

            var canonicalWordId = matchingWordIds[0];

            // Prevent duplicates by meaning (term + language)
            var userWordExists = await _context.UserWords
                .AnyAsync(uw =>
                    uw.UserId == userId &&
                    matchingWordIds.Contains(uw.WordId));

            if (userWordExists)
            {
                return DuplicateWordConflict();
            }

            // Create new UserWord
            var userWord = new UserWord(userId, canonicalWordId);
            _context.UserWords.Add(userWord);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUserWordUniqueViolation(ex))
            {
                return DuplicateWordConflict();
            }

            return NoContent();
        }

        private IActionResult DuplicateWordConflict()
        {
            return Conflict(new ProblemDetails
            {
                Title = "Word already assigned",
                Detail = "This word is already in the user's list.",
                Status = StatusCodes.Status409Conflict
            });
        }

        private static string NormaliseTerm(string term)
        {
            return term.Trim().ToLowerInvariant();
        }

        private static string NormaliseLanguageCode(string languageCode)
        {
            return languageCode.Trim().ToLowerInvariant();
        }

        private static bool IsUserWordUniqueViolation(DbUpdateException exception)
        {
            if (exception.InnerException is not PostgresException pgException)
            {
                return false;
            }

            if (pgException.SqlState != PostgresErrorCodes.UniqueViolation)
            {
                return false;
            }

            if (string.Equals(pgException.TableName, "UserWords", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return string.Equals(pgException.ConstraintName, "PK_UserWords", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(pgException.ConstraintName, "IX_UserWords_UserId_WordId", StringComparison.OrdinalIgnoreCase);
        }

    }
}
