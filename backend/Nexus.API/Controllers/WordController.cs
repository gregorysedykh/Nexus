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
            var word = _mapper.Map<Word>(createWordDto);
            _context.Words.Add(word);
            await _context.SaveChangesAsync();

            var wordDto = _mapper.Map<WordDto>(word);
            return StatusCode(StatusCodes.Status201Created, wordDto);
        }
        
    }
}
