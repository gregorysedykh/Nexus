namespace Nexus.API.DTOs;

public class CreateWordDto
{
    public string Term { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}