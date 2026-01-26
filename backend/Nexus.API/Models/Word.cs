namespace Nexus.API.Models
{
    public class Word
    {
        public int Id { get; set; }
        public string Term { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;

        public Word(string term, string languageCode)
        {
            Term = term;
            LanguageCode = languageCode;
        }
    }
}