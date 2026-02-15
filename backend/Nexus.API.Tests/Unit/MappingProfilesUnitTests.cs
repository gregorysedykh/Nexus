using AutoMapper;
using Nexus.API.DTOs;
using Nexus.API.Models;
using Nexus.API.Profiles;

namespace Nexus.API.Tests.Unit;

public sealed class MappingProfilesUnitTests
{
    private readonly IMapper _mapper;

    public MappingProfilesUnitTests()
    {
        var configuration = new MapperConfiguration(config =>
        {
            config.AddProfile<UserProfile>();
            config.AddProfile<WordProfile>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Map_CreateUserDto_To_User()
    {
        var input = new CreateUserDto
        {
            Username = "alice",
            Email = "alice@example.com"
        };

        var user = _mapper.Map<User>(input);

        Assert.Equal(input.Username, user.Username);
        Assert.Equal(input.Email, user.Email);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Map_CreateWordDto_To_Word()
    {
        var input = new CreateWordDto
        {
            Term = "Park",
            LanguageCode = "en"
        };

        var word = _mapper.Map<Word>(input);

        Assert.Equal(input.Term, word.Term);
        Assert.Equal(input.LanguageCode, word.LanguageCode);
    }
}
