namespace Nexus.API.DTOs;

public class WordDto
{
    public int Id { get; set; }
    public string Term { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}