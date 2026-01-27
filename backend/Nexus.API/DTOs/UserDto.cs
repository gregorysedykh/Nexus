namespace Nexus.API.DTOs;

public class UserDto
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;

    public ICollection<UserWord> UserWords { get; set; } = new List<UserWord>();
}