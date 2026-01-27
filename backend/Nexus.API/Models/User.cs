using System.ComponentModel.DataAnnotations;

namespace Nexus.API.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string Email { get; set; } = string.Empty;

    public ICollection<UserWord> UserWords { get; set; } = new List<UserWord>();

    public User(string username, string email)
    {
        Username = username;
        Email = email;
    }
}
