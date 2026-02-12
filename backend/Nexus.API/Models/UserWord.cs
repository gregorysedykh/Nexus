using Nexus.API.Models;

public class UserWord
{
    public int UserId { get; set; }
    public int WordId { get; set; }

    public User User { get; set; } = null!;
    public Word Word { get; set; } = null!;
    
    public UserWord(int userId, int wordId)
    {
        UserId = userId;
        WordId = wordId;
    }
}
