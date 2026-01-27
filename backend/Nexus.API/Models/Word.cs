using System.ComponentModel.DataAnnotations;

namespace Nexus.API.Models
{
    public class Word
    {
        public int Id { get; set; }

        [Required]
        public string Term { get; set; } = string.Empty;
        
        [Required]
        public string LanguageCode { get; set; } = string.Empty;

        public Word(string term, string languageCode)
        {
            Term = term;
            LanguageCode = languageCode;
        }
    }
}