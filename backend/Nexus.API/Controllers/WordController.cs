using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Nexus.API.Data;
using Nexus.API.Models;
using Nexus.API.DTOs;

namespace Nexus.API.Controllers
{
    [Route("api/words")]
    [ApiController]
    public class WordController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WordController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WordDto>>> GetWords()
        {
            var words = await _context.Words.ToListAsync();
            var wordsDto = _mapper.Map<IEnumerable<WordDto>>(words);
            return Ok(wordsDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWord(CreateWordDto createWordDto)
        {
            var normalizedTerm = NormaliseTerm(createWordDto.Term);
            var normalizedLanguageCode = NormaliseLanguageCode(createWordDto.LanguageCode);

            if (string.IsNullOrWhiteSpace(normalizedTerm) || string.IsNullOrWhiteSpace(normalizedLanguageCode))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid word payload",
                    Detail = "Term and languageCode are required.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var existingWord = await _context.Words
                .Where(w =>
                    w.Term.Trim().ToLower() == normalizedTerm.ToLower() &&
                    w.LanguageCode.Trim().ToLower() == normalizedLanguageCode)
                .OrderBy(w => w.Id)
                .FirstOrDefaultAsync();

            if (existingWord != null)
            {
                var existingWordDto = _mapper.Map<WordDto>(existingWord);
                return Ok(existingWordDto);
            }

            var word = _mapper.Map<Word>(createWordDto);
            word.Term = normalizedTerm;
            word.LanguageCode = normalizedLanguageCode;
            _context.Words.Add(word);
            await _context.SaveChangesAsync();

            var wordDto = _mapper.Map<WordDto>(word);
            return StatusCode(StatusCodes.Status201Created, wordDto);
        }

        private static string NormaliseTerm(string term)
        {
            return term.Trim();
        }

        private static string NormaliseLanguageCode(string languageCode)
        {
            return languageCode.Trim().ToLowerInvariant();
        }
        
    }
}
