using System.ComponentModel.DataAnnotations;

namespace Nexus.API.DTOs;

public class CreateWordDto
{
    [Required]
    public string Term { get; set; } = string.Empty;
    
    [Required]
    public string LanguageCode { get; set; } = string.Empty;
}