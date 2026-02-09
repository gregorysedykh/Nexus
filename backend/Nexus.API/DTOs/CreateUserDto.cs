using System.ComponentModel.DataAnnotations;

namespace Nexus.API.DTOs;

public class CreateUserDto
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

}